using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace AuctionHouse.Common.Models
{
    public class BonusList
    {
        [JsonProperty("bonusListId")]
        public long Id { get; set; }
    }

    public class Modifier
    {
        public long Type { get; set; }
        public long Value { get; set; }
    }

    public class Auction
    {
        [JsonProperty("auc"), JsonRequired]
        public long Id { get; set; }

        [JsonProperty("item"), JsonRequired]
        public int ItemId { get; set; }

        [JsonRequired]
        public string OwnerRealm { get; set; }

        [JsonRequired]
        public long Bid { get; set; }

        [JsonRequired]
        public long Buyout { get; set; }

        [JsonRequired]
        public int Quantity { get; set; }

        [JsonRequired]
        public string TimeLeft { get; set; }

        [JsonRequired]
        public int Rand { get; set; }

        [JsonRequired]
        public int Seed { get; set; }

        [JsonRequired]
        public int Context { get; set; }

        public List<BonusList> BonusLists { get; set; } = new List<BonusList>();
        public List<Modifier> Modifiers { get; set; } = new List<Modifier>();

        //[JsonExtensionData(WriteData = true, ReadData = true)]
        public Dictionary<string, long> AdditionalFields { get; set; } = new Dictionary<string, long>();

        public override bool Equals(object obj)
        {
            return obj is Auction auction &&
                   Id == auction.Id &&
                   ItemId == auction.ItemId &&
                   OwnerRealm == auction.OwnerRealm &&
                   Bid == auction.Bid &&
                   Buyout == auction.Buyout &&
                   Quantity == auction.Quantity &&
                   TimeLeft == auction.TimeLeft &&
                   Rand == auction.Rand &&
                   Seed == auction.Seed &&
                   Context == auction.Context &&
                   EqualityComparer<List<BonusList>>.Default.Equals(BonusLists, auction.BonusLists) &&
                   EqualityComparer<List<Modifier>>.Default.Equals(Modifiers, auction.Modifiers) &&
                   EqualityComparer<Dictionary<string, long>>.Default.Equals(AdditionalFields, auction.AdditionalFields);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Id);
            hash.Add(ItemId);
            hash.Add(OwnerRealm);
            hash.Add(Bid);
            hash.Add(Buyout);
            hash.Add(Quantity);
            hash.Add(TimeLeft);
            hash.Add(Rand);
            hash.Add(Seed);
            hash.Add(Context);
            hash.Add(BonusLists);
            hash.Add(Modifiers);
            hash.Add(AdditionalFields);
            return hash.ToHashCode();
        }
    }
}
