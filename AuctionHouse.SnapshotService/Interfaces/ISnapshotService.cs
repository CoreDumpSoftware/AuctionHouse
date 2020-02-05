using AuctionHouse.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionHouse.SnapshotService.Interfaces
{
    public interface ISnapshotService
    {
        public Task<IDictionary<DateTime, IEnumerable<Auction>>> GetAuctionData(string realmSlug, long itemId, DateTime start, DateTime end);
    }
}
