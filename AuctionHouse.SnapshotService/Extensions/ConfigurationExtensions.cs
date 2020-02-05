using AuctionHouse.Common.Configuration;
using AuctionHouse.Common.Extensions;
using Microsoft.Extensions.Configuration;

namespace AuctionHouse.SnapshotService.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddServiceConfigurations(this IConfigurationBuilder builder)
        {
            builder.AddCommonSecrets("/secrets/ahsecrets/", false);
            builder.Add(new KeyVaultMountedVolumeConfigurationSource("/secrets/ahsecrets/snapshot-service/", false));
            return builder;
        }
    }
}
