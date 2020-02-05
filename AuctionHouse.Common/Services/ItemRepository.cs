using AuctionHouse.Common.Interfaces;
using AuctionHouse.Common.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Services
{
    public class ItemRepository : IWowItemRepository
    {
        IMongoCollection<WowItem> collection;

        public ItemRepository(string host)
        {
            var client = new MongoClient(new MongoClientSettings
            {
                Server = new MongoServerAddress(host)
            });
            var db = client.GetDatabase("wow");
            collection = db.GetCollection<WowItem>("items");
        }

        public async Task AddItemAsync(WowItem item)
        {
            await collection.FindOneAndDeleteAsync(GetFilter().Eq("Id", item.Id));
            await collection.InsertOneAsync(item);
        }

        public async Task AddItemsAsync(IEnumerable<WowItem> items)
        {
            var ids = items.Select(i => i.Id).ToList();
            await collection.DeleteManyAsync(GetFilter().In(i => i.Id, ids));
            await collection.InsertManyAsync(items);
        }

        public async Task<IEnumerable<WowItem>> GetItemsByFTSAsync(string search)
        {
            var filter = GetFilter().Text(search);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<WowItem> GetItemAsync(int itemId)
        {
            return await collection.Find(GetFilter().Eq("Id", itemId)).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<WowItem>> GetItemsAsync(IEnumerable<int> itemId)
        {
            return await collection.AsQueryable().Where(item => itemId.Contains(item.Id)).ToListAsync();
        }

        public async Task<IEnumerable<WowItem>> GetItemsAsync(int startItemId, int endItemId)
        {
            var filter = GetFilter();
            var gte = filter.Gte("Id", startItemId);
            var lte = filter.Lte("Id", endItemId);
            var items = await collection.Find(gte & lte).ToListAsync();
            return items;
        }

        private FilterDefinitionBuilder<WowItem> GetFilter() => Builders<WowItem>.Filter;
    }
}
