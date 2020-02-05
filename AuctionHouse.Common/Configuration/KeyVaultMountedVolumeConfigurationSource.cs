using Microsoft.Extensions.Configuration;

namespace AuctionHouse.Common.Configuration
{
    public class KeyVaultMountedVolumeConfigurationSource : IConfigurationSource
    {
        private readonly string _secretsPath;
        private readonly bool _optional;

        /// <summary>
        /// Creates configuration source for reading KeyVault secrets from folder specified in path.
        /// </summary>
        /// <param name="secretsPath">Path to folder containting secrets.</param>
        /// <param name="optional">Flag indicating wether secrets are optional or not.</param>
        public KeyVaultMountedVolumeConfigurationSource(string secretsPath, bool optional)
        {
            this._secretsPath = secretsPath;
            this._optional = optional;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => new KeyVaultMountedVolumeConfigurationProvider(_secretsPath, _optional);
    }
}
