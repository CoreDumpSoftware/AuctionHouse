using System.Threading.Tasks;

namespace AuctionHouse.Common.Interfaces
{
    public interface IPopulator<T>
    {
        Task CleanAsync();
        Task PopulateAsync();
    }
}
