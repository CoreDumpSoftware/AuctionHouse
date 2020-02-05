using Newtonsoft.Json;

namespace AuctionHouse.Common.Models
{
    public class BlizzardSecrets
    {
        [JsonProperty("apiBaseUrl")]
        public string ApiBaseUrl { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; }

        [JsonProperty("tokenBaseUrl")]
        public string TokenBaseUrl { get; set; }
    }
}
