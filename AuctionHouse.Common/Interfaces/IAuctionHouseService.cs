using AuctionHouse.Common.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Interfaces
{
    public interface IAuctionHouseService
    {
        Task<AuctionHouseSnapshotMetadata> GetSnapshotMetadataAsync(Realm realm, CancellationToken stoppingToken);
        Task<AuctionHouseSnapshot> GetSnapshotAsync(AuctionHouseSnapshotMetadata metadata, CancellationToken stoppingToken);
    }
}
