using AuctionHouse.Common.Interfaces;
using AuctionHouse.Common.Models;
using AuctionHouse.SnapshotService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse.SnapshotService.Services
{
    public class SnapshotFetcher : BackgroundService
    {
        private readonly IAuctionHouseService _service;
        private readonly IEnumerable<Common.Models.Realm> _realms;
        private readonly SnapshotDbContext _dbContext;

        private Dictionary<Common.Models.Realm, long> lastTimestampByRealms = new Dictionary<Common.Models.Realm, long>();
        private IDictionary<string, NameDetail> dbNames = new ConcurrentDictionary<string, NameDetail>();
        private IDictionary<string, Models.Realm> dbRealms = new ConcurrentDictionary<string, Models.Realm>();

        public SnapshotFetcher(IAuctionHouseService auctionHouseService,
            IEnumerable<Common.Models.Realm> realms,
            SnapshotDbContext dbContext)
        {
            _service = auctionHouseService;
            _realms = realms;
            _dbContext = dbContext;

            _dbContext.Database.EnsureCreated();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested == false)
            {
                foreach (var realm in _realms)
                {
                    await GetSnapshotForRealmAsync(realm, stoppingToken);
                }

                stoppingToken.WaitHandle.WaitOne(TimeSpan.FromMinutes(5));
            }
        }

        private async Task GetSnapshotForRealmAsync(Common.Models.Realm realm, CancellationToken stoppingToken)
        {
            var metadata = await _service.GetSnapshotMetadataAsync(realm, stoppingToken);
            var timestamp = (long)(metadata.Files.First().LastModified - DateTime.UnixEpoch).TotalMilliseconds;

            if (!lastTimestampByRealms.TryGetValue(realm, out var lastTimestamp))
            {
                lastTimestampByRealms.Add(realm, timestamp);
            }
            else if (timestamp <= lastTimestamp)
            {
                return;
            }
            else
            {
                lastTimestampByRealms[realm] = timestamp;
            }

            var snapshot = await _service.GetSnapshotAsync(metadata, stoppingToken);
            Console.WriteLine($"Inserting auctions for {realm.Name}");
            await InsertSnapshotToDbAsync(snapshot, stoppingToken);
            Console.WriteLine("Done!");
        }

        private async Task InsertSnapshotToDbAsync(AuctionHouseSnapshot snapshot, CancellationToken stoppingToken)
        {
            try
            {
                var snapshotEntry = new Snapshot
                {
                    Timestamp = snapshot.Timestamp
                };

                snapshotEntry = await AddAsync(_dbContext.Snapshots, snapshotEntry);
                await _dbContext.SaveChangesAsync(stoppingToken);

                await foreach (var realm in FindOrInsertRealmAsync(snapshot.Realms, stoppingToken))
                {
                    await InsertSnapshotRealmDetailAsync(snapshotEntry.Id, realm, stoppingToken);
                }

                var existingAdditionalFields = await _dbContext.NameDetails.Select(d => d.Name).ToListAsync(stoppingToken);
                var additionalFields = snapshot.Auctions.SelectMany(a => a.AdditionalFields)
                    .Select(a => a.Key)
                    .Distinct()
                    .Where(a => !existingAdditionalFields.Contains(a))
                    .Select(n => new NameDetail
                    {
                        Name = n
                    })
                    .ToList();
                _dbContext.NameDetails.AddRange(additionalFields);
                await _dbContext.SaveChangesAsync(stoppingToken);

                // need to pre-load these and then update distinct entries
                var nameDetails = _dbContext.NameDetails.Select(d => new KeyValuePair<string, NameDetail>(d.Name, d));
                dbNames = new ConcurrentDictionary<string, NameDetail>(nameDetails);
                var realms = _dbContext.Realms.Select(r => new KeyValuePair<string, Models.Realm>(r.Name, r));
                dbRealms = new ConcurrentDictionary<string, Models.Realm>(realms);

                await InsertAuctionsAsync(snapshotEntry.Id, snapshot.Auctions, stoppingToken);
                //await InsertAuctionDetailsAsync(snapshot.Auctions, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task<List<Models.Auction>> InsertAuctionsAsync(long snapshotId, List<Common.Models.Auction> auctions, CancellationToken stoppingToken)
        {
            long currentAuctionId = -1;
            try
            {
                foreach (var auction in auctions)
                {
                    currentAuctionId = auction.Id;
                    if (string.IsNullOrEmpty(auction.OwnerRealm))
                    {
                        continue;
                    }

                    var realm = dbRealms[auction.OwnerRealm];


                    var entity = new Models.Auction
                    {
                        AuctionId = auction.Id,
                        Bid = auction.Bid,
                        Buyout = auction.Buyout,
                        Context = auction.Context,
                        ItemId = auction.ItemId,
                        OwnerRealm = realm,
                        OwnerRealmId = realm.Id,
                        Quantity = auction.Quantity,
                        Rand = auction.Rand,
                        Seed = auction.Seed,
                        SnapshotId = snapshotId,
                        TimeLeft = TimeLeftConverter.ToEnum(auction.TimeLeft)
                    };

                    var bonusLists = auction.BonusLists.Select(d => new BonusListDetail
                    {
                        Auction = entity,
                        AuctionId = entity.AuctionId,
                        BonusListId = d.Id
                    }).ToList();
                    var modifiers = auction.Modifiers.Select(d => new ModifierDetail
                    {
                        Auction = entity,
                        AuctionId = entity.AuctionId,
                        Type = d.Type,
                        Value = d.Value
                    }).ToList(); ;
                    var additionFields = auction.AdditionalFields.Select(d =>
                    {
                        var nameDetail = dbNames.First(n => n.Key == d.Key).Value;
                        return new AdditionalFieldDetail
                        {
                            Auction = entity,
                            AuctionId = entity.AuctionId,
                            NameDetail = nameDetail,
                            NameDetailId = nameDetail.Id,
                            Value = d.Value
                        };
                    }).ToList();

                    entity.AdditionalFieldDetails = additionFields;
                    entity.BonusListDetails = bonusLists;
                    entity.ModifierDetails = modifiers;

                    await AddAsync(_dbContext.Auctions, entity);
                }

                await _dbContext.SaveChangesAsync(stoppingToken);
                return await _dbContext.Auctions.ToListAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuctionId: {currentAuctionId}");
                throw ex;
            }
        }

        private async Task InsertAuctionDetailsAsync(List<Common.Models.Auction> auctions, CancellationToken stoppingToken)
        {
            foreach (var auction in auctions)
            {
                var bonusLists = auction.BonusLists.Select(d => new BonusListDetail
                {
                    AuctionId = auction.Id,
                    BonusListId = d.Id
                });
                var modifiers = auction.Modifiers.Select(d => new ModifierDetail
                {
                    AuctionId = auction.Id,
                    Type = d.Type,
                    Value = d.Value
                });
                var additionFields = auction.AdditionalFields.Select(d => new AdditionalFieldDetail
                {
                    AuctionId = auction.Id,
                    NameDetailId = dbNames.First(n => n.Key == d.Key).Value.Id,
                    Value = d.Value
                });

                await _dbContext.BonusListDetails.AddRangeAsync(bonusLists, stoppingToken);
                await _dbContext.ModifierDetails.AddRangeAsync(modifiers, stoppingToken);
                await _dbContext.AdditionalFieldDetails.AddRangeAsync(additionFields, stoppingToken);
            }

            await _dbContext.SaveChangesAsync(stoppingToken);
        }

        private async IAsyncEnumerable<Models.Realm> FindOrInsertRealmAsync(List<Common.Models.Realm> realms, [EnumeratorCancellation] CancellationToken stoppingToken)
        {
            foreach (var realm in realms)
            {
                var realmEntity = await FindRealmAsync(realm, stoppingToken);
                if (realmEntity == null)
                {
                    realmEntity = await InsertRealmAsync(realm, stoppingToken);
                }

                yield return realmEntity;
            }
        }

        private async Task InsertSnapshotRealmDetailAsync(long snapshotId, Models.Realm realm, CancellationToken stoppingToken)
        {
            var entry = new SnapshotRealm
            {
                SnapshotId = snapshotId,
                RealmId = realm.Id
            };

            await _dbContext.AddAsync(entry, stoppingToken);
        }

        private async Task<Models.Realm> FindRealmAsync(Common.Models.Realm realm, CancellationToken stoppingToken)
        {
            try
            {
                return await _dbContext.Realms.FirstOrDefaultAsync(r => r.Name == realm.Name && r.Slug == realm.Slug, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private async Task<Models.Realm> InsertRealmAsync(Common.Models.Realm realm, CancellationToken stoppingToken)
        {
            var entry = new Models.Realm
            {
                Name = realm.Name,
                Slug = realm.Slug
            };

            var result = await AddAsync(_dbContext.Realms, entry);
            _dbContext.SaveChanges();
            return _dbContext.Realms.First(r => r.Name == entry.Name);
        }

        private async Task<T> AddAsync<T>(DbSet<T> set, T item) where T: class
        {
            return (await set.AddAsync(item)).Entity;
        }
    }
}
