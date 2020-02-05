using AuctionHouse.Common.Interfaces;
using AuctionHouse.Common.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace AuctionHouse.Common.Services
{
    public class TokenHandler : ITokenHandler
    {
        private static readonly TimeSpan tokenExpirationOffset = TimeSpan.FromMinutes(15);
        private readonly IAuthService _authService;
        private DateTime refreshAt = DateTime.UnixEpoch;
        private OAuthToken token;
        private System.Timers.Timer refreshTimer;

        public TokenHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken stoppingToken, bool refresh = false)
        {
            if (refresh || token == null || DateTime.Now >= refreshAt)
            {
                token = await _authService.GetAccessTokenAsync(stoppingToken);
                refreshAt = GetRefreshDateTime(token);
                refreshTimer?.Dispose();
                refreshTimer = new System.Timers.Timer((refreshAt - DateTime.Now).TotalMilliseconds)
                {
                    AutoReset = false,
                    Enabled = true
                };
                refreshTimer.Elapsed += async (s, a) =>
                {
                    await GetAccessTokenAsync(stoppingToken, true);
                };
            }

            return token.AccessToken;
        }

        private DateTime GetRefreshDateTime(OAuthToken token)
        {
            var now = DateTime.Now;
            return now.AddSeconds(token.ExpiresIn).Subtract(tokenExpirationOffset);
        }
    }
}
