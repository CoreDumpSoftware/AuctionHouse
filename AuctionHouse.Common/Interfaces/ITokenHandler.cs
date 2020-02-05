using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Interfaces
{
    public interface ITokenHandler
    {
        Task<string> GetAccessTokenAsync(CancellationToken stoppingToken, bool refresh = false);
    }
}
