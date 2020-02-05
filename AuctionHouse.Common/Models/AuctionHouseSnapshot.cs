using System.Collections.Generic;

namespace AuctionHouse.Common.Models
{
    public class AuctionHouseSnapshot
    {
        public List<Realm> Realms { get; set; }
        public List<Auction> Auctions { get; set; }
        public long Timestamp { get; set; }
    }
}
