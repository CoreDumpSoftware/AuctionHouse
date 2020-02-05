using Newtonsoft.Json;
using System;

namespace AuctionHouse.Common.Models
{
    public class AuctionFile
    {
        public string Url { get; set; }

        [JsonProperty("lastModified")]
        private long lastModified;

        public DateTime LastModified => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(lastModified);
    }
}
