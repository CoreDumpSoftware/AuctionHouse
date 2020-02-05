using AuctionHouse.Common.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Interfaces
{
    public interface IAuthService
    {
        public Task<OAuthToken> GetAccessTokenAsync(CancellationToken stoppingToken);
    }
}
