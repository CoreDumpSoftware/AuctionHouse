using Newtonsoft.Json;
using System.Collections.Generic;

namespace AuctionHouse.Common.Models
{
    public class WowItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("quality")]
        public int QualityId { get; set; }

        [JsonProperty("classs")]
        public int ClassId { get; set; }

        [JsonProperty("subclass")]
        public int SubclassId { get; set; }

        public Icon Icon { get; set; }


        [JsonExtensionData()]
        private Dictionary<string, object> otherValues { get; set; }
    }
}
