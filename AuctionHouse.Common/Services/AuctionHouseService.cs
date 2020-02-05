using AuctionHouse.Common.Interfaces;
using AuctionHouse.Common.Models;
using Flurl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Services
{
    public class AuctionHouseService : IAuctionHouseService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenHandler _tokenHandler;
        private readonly string _apiUrl;

        public AuctionHouseService(HttpClient httpClient, ITokenHandler tokenHandler, string apiUrl)
        {
            _apiUrl = apiUrl;
            _httpClient = httpClient;
            _tokenHandler = tokenHandler;
        }

        public async Task<AuctionHouseSnapshotMetadata> GetSnapshotMetadataAsync(Realm realm, CancellationToken stoppingToken)
        {
            var url = await GetAuctionDataUrlAsync(realm, stoppingToken);
            var response = await _httpClient.GetAsync(url, stoppingToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var metadata = JsonConvert.DeserializeObject<AuctionHouseSnapshotMetadata>(jsonContent);
            return metadata;
        }

        public async Task<AuctionHouseSnapshot> GetSnapshotAsync(AuctionHouseSnapshotMetadata metadata, CancellationToken stoppingToken)
        {
            AuctionHouseSnapshot snapshot = null;

            if (metadata.Files.Count > 1)
            {
                var snapshots = new List<AuctionHouseSnapshot>();
                Parallel.ForEach(metadata.Files, async file =>
                {
                    var snapshot = await DownloadSnapshotAsync(file.Url, stoppingToken);
                    snapshots.Add(snapshot);
                });

                var aggregate = new AuctionHouseSnapshot();
                aggregate.Realms = snapshots.SelectMany(s => s.Realms)
                    .Distinct()
                    .ToList();
                aggregate.Auctions = snapshots.SelectMany(s => s.Auctions).ToList();
                snapshot = aggregate;
            }
            else
            {
                snapshot = await DownloadSnapshotAsync(metadata.Files.First().Url, stoppingToken);
            }

            snapshot.Timestamp = (long)(metadata.Files.First().LastModified - DateTime.UnixEpoch).TotalMilliseconds;
            return snapshot;
        }

        private async Task<AuctionHouseSnapshot> DownloadSnapshotAsync(string url, CancellationToken stoppingToken)
        {
            var response = await _httpClient.GetAsync(url, stoppingToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            var document = JObject.Parse(json);

            return ParseSnapshot(document);
        }

        private async Task<string> GetAuctionDataUrlAsync(Realm realm, CancellationToken stoppingToken)
        {
            string path = $"/wow/auction/data/{realm.Slug}";
            var url = new Url(Url.Combine(_apiUrl, path));
            url.SetQueryParam("locale", "en_US");
            var accessToken = await _tokenHandler.GetAccessTokenAsync(stoppingToken);
            url.SetQueryParam("access_token", $"{accessToken}");

            return url.ToString();
        }

        private AuctionHouseSnapshot ParseSnapshot(JObject document)
        {
            var snapshot = new AuctionHouseSnapshot();
            foreach (var token in document.Children<JProperty>())
            {
                var items = token.Children().Children();
                if (token.Name == "realms")
                {
                    snapshot.Realms = ParseRealms(items).ToList();
                }
                else if (token.Name == "auctions")
                {
                    snapshot.Auctions = ParseAuctions(items).ToList();
                }
            }

            return snapshot;
        }

        private IEnumerable<Realm> ParseRealms(IEnumerable<JToken> tokens)
        {
            foreach (var token in tokens)
            {
                yield return token.ToObject<Realm>();
            }
        }

        private IEnumerable<Auction> ParseAuctions(IEnumerable<JToken> tokens)
        {
            foreach (var token in tokens)
            {
                var auction = new Auction();
                foreach (var property in token.Children<JProperty>())
                {
                    switch (property.Name)
                    {
                        case "auc":
                            auction.Id = property.ToObject<long>();
                            break;
                        case "item":
                            auction.ItemId = property.ToObject<int>();
                            break;
                        case "ownerRealm":
                            auction.OwnerRealm = property.ToObject<string>();
                            break;
                        case "bid":
                            auction.Bid = property.ToObject<long>();
                            break;
                        case "buyout":
                            auction.Buyout = property.ToObject<long>();
                            break;
                        case "quantity":
                            auction.Quantity = property.ToObject<int>();
                            break;
                        case "timeLeft":
                            auction.TimeLeft = property.ToObject<string>();
                            break;
                        case "rand":
                            auction.Rand = property.ToObject<int>();
                            break;
                        case "seed":
                            auction.Seed = property.ToObject<int>();
                            break;
                        case "context":
                            auction.Context = property.ToObject<int>();
                            break;
                        case "bonusLists":
                            auction.BonusLists = ParseBonusLists(property.Children<JArray>().First().Children()).ToList();
                            break;
                        case "modifiers":
                            auction.Modifiers = ParseModifier(property.Children<JArray>().First().Children()).ToList();
                            break;
                        default:
                            auction.AdditionalFields.Add(property.Name, property.ToObject<long>());
                            break;
                    }
                }

                yield return auction;
            }
        }

        private IEnumerable<BonusList> ParseBonusLists(IEnumerable<JToken> tokens)
        {
            foreach (var token in tokens)
            {
                yield return token.ToObject<BonusList>();
            }
        }

        private IEnumerable<Modifier> ParseModifier(IEnumerable<JToken> tokens)
        {
            foreach (var token in tokens)
            {
                yield return token.ToObject<Modifier>();
            }
        }
    }
}
