using AuctionHouse.Common.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestBed
{
    public static class Extensions
    {
        public static IMongoQueryable<T> WhereText<T>(this IMongoQueryable<T> query, string search)
        {
            var filter = Builders<T>.Filter.Text(search);
            return query.Where(_ => filter.Inject());
        }
    }

    public class Range
    {
        public int StartAt;
        public int EndAt;
    }

    class Program
    {
        private static List<Range> NumberToRanges(int number, int rangeCount)
        {
            var countPerRange = number / rangeCount + (number % rangeCount == 0 ? 0 : 1);
            var ranges = new List<Range>(rangeCount);

            for (var i = 0; i < rangeCount; i++)
            {
                ranges.Add(new Range
                {
                    StartAt = countPerRange * i,
                    EndAt = countPerRange * (i + 1)
                });
            }

            return ranges;
        }

        static async Task Main(string[] args)
        {
            var timedString = new Timed<string>("Hello World!", DateTime.Now.AddSeconds(15));
            var stoppingTokenSource = new CancellationTokenSource();
            timedString.OnExpiration += (timedObject) =>
            {
                stoppingTokenSource.Cancel();
            };

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingTokenSource.Token);
            stoppingTokenSource.Dispose();


            Console.WriteLine("Timer has expired.");

            //var id = "a67e45343c4c49f1a59c3a8ae8e32ffb";
            //var secret = "OfREp95eHBg7SF1J8M9QZSx1vojwUSfS";
            //var tokenBaseUrl = "https://us.battle.net";
            //var apiBaseUrl = "https://us.api.blizzard.com";
            //var wowheadUrl = "https://wowhead.com";

            //using var httpClient = new HttpClient();
            //var authService = new AuthService(httpClient, tokenBaseUrl, id, secret);
            //var tokenHandler = new TokenHandler(authService);
            //var token = await tokenHandler.GetAccessTokenAsync();

            //var auctionHouseService = new AuctionHouseService(httpClient, tokenHandler, apiBaseUrl);
            //var realm = new Realm
            //{
            //    Name = "Area 52",
            //    Slug = "area-52"
            //};
            //var snapshot = await auctionHouseService.GetSnapshotAsync(realm);


            //var iconFactory = new IconFactory();
            //var wowheadService = new WowheadService(httpClient, iconFactory, "https://wowhead.com");

            //var client = new MongoClient(new MongoClientSettings
            //{
            //    Server = new MongoServerAddress("192.168.0.103")
            //});
            //var wowDb = client.GetDatabase("wow");
            //var collection = wowDb.GetCollection<WowItem>("items");
            ////var indexOptions = new CreateIndexOptions();
            ////var indexKeys = Builders<WowItem>.IndexKeys.Text(i => i.Name);
            ////var indexModel = new CreateIndexModel<WowItem>(indexKeys, indexOptions);
            ////await collection.Indexes.CreateOneAsync(indexModel);

            //var foundItems = collection.AsQueryable().Where(i => i.Name.Contains("bar", StringComparison.OrdinalIgnoreCase)); //collection.AsQueryable().WhereText("Bar").ToList();

            ////var filterBuilder = Builders<WowItem>.Filter;
            ////var filter = filterBuilder.Text("Arcanite");
            ////var foundItems = await collection.Find(filter).ToListAsync();
            //foreach (var x in foundItems)
            //{
            //    Console.WriteLine($"{x.Id}: {x.Name}");
            //    Console.WriteLine();
            //}

            //var repo = new ItemRepository("192.168.0.103");
            //var allItems = (await repo.GetItemsAsync(0, 500000)).Where(i => i.Name != NullWowItem.NullItemName).ToList();
            //var allThatExist = allItems.Where(i => i.Name != NullWowItem.NullItemName || i.Name.StartsWith("OLD", StringComparison.OrdinalIgnoreCase) || i.Name.StartsWith("Test", StringComparison.OrdinalIgnoreCase)).ToList();


            //var updatedItems = new List<WowItem>();
            //var threadCount = 12;
            //var ranges = NumberToRanges(allItems.Count(), threadCount);
            //var httpClients = new List<HttpClient>();
            //var services = new List<IWowheadService>();
            //var tasks = new List<Task>();
            //for (var i = 0; i < threadCount; i++)
            //{
            //    //httpClients.Add(new HttpClient());
            //    //services.Add(new WowheadService(httpClients[i], iconFactory, wowheadUrl));
            //    var client = new HttpClient();
            //    var service = new WowheadService(client, iconFactory, wowheadUrl);
            //    var range = ranges[i];
            //    tasks.Add(Task.Run(async () =>
            //    {
            //        var _httpClient = httpClient;
            //        var _service = service;
            //        var _range = range;

            //        for (var i = _range.StartAt; i < _range.EndAt; i++)
            //         {
            //            var item = await _service.GetItemAsync(i);
            //            await repo.AddItemAsync(item);
            //        }

            //        _httpClient.Dispose();
            //    }));
            //}

            //await Task.WhenAll(tasks);
            //repo.AddItemsAsync(updatedItems);

            //Parallel.ForEach(allItems, new ParallelOptions { MaxDegreeOfParallelism = 10 }, async item =>
            //{
            //    var updatedItem = await wowheadService.GetItemAsync(item.Id);
            //    updatedItems.Add(updatedItem);
            //});

            //await repo.AddItemsAsync(updatedItems);
            //foreach (var item in allThatExist)
            //{
            //    Console.WriteLine($"{item.Id}: {item.Name}");
            //}
            //var clothIds = new[] { 4306, 21877, 33470, 4338, 53010, 2592, 2589, 72988 };
            ////var clothItems = await repo.GetItemsAsync(new[] { 4306, 21877, 33470, 4338, 53010, 2592, 2589, 72988 });

            //var items = new List<WowItem>();
            //foreach (var itemId in clothIds)
            //{
            //    items.Add(await wowheadService.GetItemAsync(itemId));
            //}

            //await repo.AddItemsAsync(items);
            //var ids = await repo.GetItemsAsync(clothIds);


            //var populator = new WowItemPopulator(wowheadService, repo);
            //await populator.PopulateAsync();
            //await repo.AddItemAsync(item);
            ////var item = await repo.GetItemAsync(12360);
            //var items = (await repo.GetItemsByFTSAsync("Arcanite")).ToList();
            ////Console.WriteLine(item.Name);
            //foreach (var i in items)
            //{
            //    Console.WriteLine($"{i.Id}: {i.Name}");
            //}

            //var count = 30;
            //var httpClients = new List<HttpClient>();
            //var services = new List<WowheadService>();
            //for (var i = 0; i < count; i++)
            //{
            //    var httpClient = new HttpClient();
            //    httpClients.Add(httpClient);
            //    services.Add(new WowheadService(httpClient, wowheadUrl));
            //}

            //var populator = new ParallelWowItemPopulator(services.ToArray(), repo);
            //await populator.PopulateAsync();

            //for (var i = 0; i < count; i++)
            //{
            //    httpClients[i].Dispose();
            //}
        }
    }
}
