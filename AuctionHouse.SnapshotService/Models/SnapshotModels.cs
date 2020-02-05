using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuctionHouse.SnapshotService.Models
{
    public abstract class EntityBase
    {
        [Key]
        public long Id { get; set; }
    }

    public class Snapshot : EntityBase
    {
        public long Timestamp { get; set; }
        public virtual List<SnapshotRealm> SnapshotRealms { get; set; }
        public virtual List<Auction> Auctions { get; set; }
    }

    public class SnapshotRealm
    {
        public long SnapshotId { get; set; }
        public Snapshot Snapshot { get; set; }

        public long RealmId { get; set; }
        public Realm Realm { get; set; }
    }

    public class Realm : EntityBase
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        public virtual List<SnapshotRealm> SnapshotRealms { get; set; }
        public virtual List<Auction> Auctions { get; set; }
    }

    public class Auction : EntityBase
    {
        public long AuctionId { get; set; }
        public long SnapshotId { get; set; }
        public virtual Snapshot Snapshot { get; set; }
        public int ItemId { get; set; }
        public long OwnerRealmId { get; set; }
        public Realm OwnerRealm { get; set; }
        public long Bid { get; set; }
        public long Buyout { get; set; }
        public int Quantity { get; set; }
        public TimeLeft TimeLeft { get; set; }
        public int Rand { get; set; }
        public int Seed { get; set; }
        public int Context { get; set; }
        public virtual List<BonusListDetail> BonusListDetails { get; set; }
        public virtual List<ModifierDetail> ModifierDetails { get; set; }
        public virtual List<AdditionalFieldDetail> AdditionalFieldDetails { get; set; }
    }

    public abstract class AuctionDetailBase : EntityBase
    {
        public long AuctionId { get; set; }
        public virtual Auction Auction { get; set; }
    }

    public class BonusListDetail : AuctionDetailBase
    {
        public long BonusListId { get; set; }
    }

    public class ModifierDetail : AuctionDetailBase
    {
        public long Type { get; set; }
        public long Value { get; set; }
    }

    public class AdditionalFieldDetail : AuctionDetailBase
    {
        public long NameDetailId { get; set; }
        public NameDetail NameDetail { get; set; }
        public string Name => NameDetail?.Name;
        public long Value { get; set; }
    }

    public class NameDetail : EntityBase
    {
        public string Name { get; set; }

        public List <AdditionalFieldDetail> AdditionalFieldDetails { get; set; }
    }

    public enum TimeLeft
    {
        VeryLong,
        Long,
        Medium,
        Short
    }

    public class TimeLeftConverter
    {
        private const string VERY_LONG = nameof(VERY_LONG);
        private const string LONG = nameof(LONG);
        private const string MEDIUM = nameof(MEDIUM);
        private const string SHORT = nameof(SHORT);

        public static TimeLeft ToEnum(string timeLeftValue)
        {
            return timeLeftValue.ToUpper() switch
            {
                VERY_LONG => TimeLeft.VeryLong,
                LONG => TimeLeft.Long,
                MEDIUM => TimeLeft.Medium,
                SHORT => TimeLeft.Short,
                _ => throw new InvalidEnumArgumentException()
            };
        }

        public static string ToEnumString(TimeLeft timeLeft)
        {
            return timeLeft switch
            {
                TimeLeft.VeryLong => VERY_LONG,
                TimeLeft.Long => LONG,
                TimeLeft.Medium => MEDIUM,
                TimeLeft.Short => SHORT,
                _ => throw new InvalidEnumArgumentException()
            };
        }
    }

}
