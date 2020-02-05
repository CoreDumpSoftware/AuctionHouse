using AuctionHouse.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Interfaces
{
    public interface IWowItemRepository
    {
        Task AddItemAsync(WowItem item);
        Task AddItemsAsync(IEnumerable<WowItem> items);
        Task<WowItem> GetItemAsync(int itemId);
        Task<IEnumerable<WowItem>> GetItemsAsync(IEnumerable<int> itemId);
        Task<IEnumerable<WowItem>> GetItemsAsync(int startItemId, int endItemId);
        Task<IEnumerable<WowItem>> GetItemsByFTSAsync(string search);
    }
}
