using AuctionHouse.Common.Interfaces;
using AuctionHouse.Common.Models;
using Flurl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string basicCredentials;
        private readonly string tokenUrl;

        public AuthService(HttpClient httpClient, string apiUrl, string clientId, string clientSecret)
        {
            _httpClient = httpClient;
            _apiUrl = apiUrl;
            basicCredentials = EncodeClientCredentials(clientId, clientSecret);
            tokenUrl = GetTokenUrl(apiUrl);
        }

        public async Task<OAuthToken> GetAccessTokenAsync(CancellationToken stoppingToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicCredentials);
            var response = await _httpClient.SendAsync(request, stoppingToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            else
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OAuthToken>(jsonContent);
            }
        }

        private string GetTokenUrl(string apiUrl)
        {
            const string path = @"/oauth/token";
            var url = new Url(Url.Combine(_apiUrl, path));
            url.SetQueryParam("region", "us");
            url.SetQueryParam("grant_type", "client_credentials");
            return url.ToString();
        }

        private string EncodeClientCredentials(string clientId, string clientSecret)
        {
            var combined = $"{clientId}:{clientSecret}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(combined));
        }
    }
}
