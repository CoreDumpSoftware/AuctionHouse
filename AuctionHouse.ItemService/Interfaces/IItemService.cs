using AuctionHouse.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse.ItemService.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<WowItem>> GetItems(IEnumerable<int> itemIds);
        Task<IEnumerable<WowItem>> GetItems(string search);
        Task<WowItem> RefreshItemAsync(int itemId);
        Task RefreshDatabaseAsync();
    }
}
