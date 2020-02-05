using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionHouse.SnapshotService.Models
{
    public class SnapshotDbContext : DbContext
    {
        private string _connString;

        public virtual DbSet<Snapshot> Snapshots { get; set; }
        public virtual DbSet<Realm> Realms { get; set; }
        public virtual DbSet<SnapshotRealm> SnapshotRealms { get; set; }
        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<BonusListDetail> BonusListDetails { get; set; }
        public virtual DbSet<ModifierDetail> ModifierDetails { get; set; }
        public virtual DbSet<AdditionalFieldDetail> AdditionalFieldDetails { get; set; }
        public virtual DbSet<NameDetail> NameDetails { get; set; }

        public SnapshotDbContext(string connectionString) : base()
        {
            _connString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(_connString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ConfigureSnapshot(GetEntity<Snapshot>(builder));
            ConfigureSnapshotRealms(GetEntity<SnapshotRealm>(builder));
            ConfigureRealm(GetEntity<Realm>(builder));
            ConfigureAuction(GetEntity<Auction>(builder));
            ConfigureBonusListDetails(GetEntity<BonusListDetail>(builder));
            ConfigureModifierDetails(GetEntity<ModifierDetail>(builder));
            ConfigureAdditionalFieldDetails(GetEntity<AdditionalFieldDetail>(builder));
            ConfigureNameDetails(GetEntity<NameDetail>(builder));
        }

        private void ConfigureAuction(EntityTypeBuilder<Auction> entity)
        {
            entity.Property(a => a.Id).ValueGeneratedOnAdd();

            entity.HasMany(af => af.AdditionalFieldDetails).WithOne(a => a.Auction).HasForeignKey(af => af.AuctionId);
            entity.HasMany(bl => bl.BonusListDetails).WithOne(a => a.Auction).HasForeignKey(bl => bl.AuctionId);
            entity.HasMany(m => m.ModifierDetails).WithOne(a => a.Auction).HasForeignKey(m => m.AuctionId);

            entity.HasOne(a => a.Snapshot).WithMany(s => s.Auctions).HasForeignKey(p => p.SnapshotId);
            entity.HasOne(a => a.OwnerRealm).WithMany(r => r.Auctions).HasForeignKey(a => a.OwnerRealmId);
        }

        private void ConfigureSnapshot(EntityTypeBuilder<Snapshot> entity)
        {
            entity.Property(s => s.Id).ValueGeneratedOnAdd();

            entity.HasMany(s => s.Auctions).WithOne(a => a.Snapshot).HasForeignKey(s => s.SnapshotId);
            entity.HasMany(s => s.SnapshotRealms).WithOne(sr => sr.Snapshot).HasForeignKey(s => s.SnapshotId);
        }

        private void ConfigureRealm(EntityTypeBuilder<Realm> entity)
        {
            entity.Property(r => r.Id).ValueGeneratedOnAdd();

            entity.HasMany(r => r.SnapshotRealms).WithOne(sr => sr.Realm).HasForeignKey(sr => sr.RealmId);
            entity.HasMany(r => r.Auctions).WithOne(sr => sr.OwnerRealm).HasForeignKey(auction => auction.OwnerRealmId);
        }

        private void ConfigureSnapshotRealms(EntityTypeBuilder<SnapshotRealm> entity)
        {
            entity.HasKey(sr => new { sr.RealmId, sr.SnapshotId });
            entity.HasOne(sr => sr.Snapshot).WithMany(s => s.SnapshotRealms).HasForeignKey(sr => sr.SnapshotId);
            entity.HasOne(sr => sr.Realm).WithMany(r => r.SnapshotRealms).HasForeignKey(sr => sr.RealmId);
        }

        private void ConfigureBonusListDetails(EntityTypeBuilder<BonusListDetail> entity)
        {
            entity.HasKey(d => d.Id);

            entity.HasOne(d => d.Auction).WithMany(a => a.BonusListDetails).HasForeignKey(d => d.AuctionId);
        }

        private void ConfigureModifierDetails(EntityTypeBuilder<ModifierDetail> entity)
        {
            entity.HasKey(d => d.Id);

            entity.HasOne(d => d.Auction).WithMany(a => a.ModifierDetails).HasForeignKey(d => d.AuctionId);
        }

        private void ConfigureAdditionalFieldDetails(EntityTypeBuilder<AdditionalFieldDetail> entity)
        {
            entity.HasKey(d => d.Id);

            entity.HasOne(d => d.Auction).WithMany(a => a.AdditionalFieldDetails).HasForeignKey(d => d.AuctionId);
            entity.HasOne(d => d.NameDetail).WithMany(d => d.AdditionalFieldDetails).HasForeignKey(d => d.NameDetailId);
        }

        private void ConfigureNameDetails(EntityTypeBuilder<NameDetail> entity)
        {
            entity.HasKey(d => d.Id);

            entity.HasMany(d => d.AdditionalFieldDetails).WithOne(d => d.NameDetail).HasForeignKey(d => d.NameDetailId);
        }

        private EntityTypeBuilder<T> GetEntity<T>(ModelBuilder builder) where T: class => builder.Entity<T>();
    }
}

