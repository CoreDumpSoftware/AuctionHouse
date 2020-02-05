using AuctionHouse.Common.Models;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Interfaces
{
    public interface IWowheadService
    {
        Task<string> GetItemJsonAsync(int itemId);
        Task<WowItem> GetItemAsync(int itemId);
    }
}
