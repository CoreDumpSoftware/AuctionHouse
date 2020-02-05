using AuctionHouse.ItemService.Interfaces;
using AuctionHouse.Common.Interfaces;
using AuctionHouse.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse.ItemService.Services
{
    public class ItemService : IItemService
    {
        private readonly IWowItemRepository _repo;
        private readonly IWowheadService _wowheadService;

        public ItemService(IWowItemRepository repository, IWowheadService wowheadService)
        {
            _repo = repository;
            _wowheadService = wowheadService;
        }

        public async Task<IEnumerable<WowItem>> GetItems(IEnumerable<int> itemIds)
        {
            return await _repo.GetItemsAsync(itemIds);
        }

        public async Task<IEnumerable<WowItem>> GetItems(string search)
        {
            return await _repo.GetItemsByFTSAsync(search);
        }

        public async Task RefreshDatabaseAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<WowItem> RefreshItemAsync(int itemId)
        {
            var item = await _wowheadService.GetItemAsync(itemId);
            if (item != null)
            {
                await _repo.AddItemAsync(item);
            }

            return item;
        }
    }
}
