using Microsoft.Extensions.Configuration;

namespace AuctionHouse.Common.Configuration
{
    public static class KeyVaultMountedVolumeConfigurationExtensions
    {
        /// <summary>
        /// Adds secrets to configuration from mounted KeyVault volume.
        /// </summary>
        /// <param name="builder">Object used to build app configuration.</param>
        /// <param name="secretsPath">Path to folder containting secrets.</param>
        /// <param name="optional">Flag indicating wether secrets are optional or not.</param>
        /// <returns>Object used to build app configuration.</returns>
        public static IConfigurationBuilder AddMountedKeyVaultSecrets(this IConfigurationBuilder builder, string secretsPath, bool optional = true)
        {
            builder.Add(new KeyVaultMountedVolumeConfigurationSource(secretsPath, optional));
            return builder;
        }
    }
}
