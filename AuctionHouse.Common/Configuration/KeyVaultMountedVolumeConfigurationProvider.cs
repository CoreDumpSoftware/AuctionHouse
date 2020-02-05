using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AuctionHouse.Common.Configuration
{
    public class KeyVaultMountedVolumeConfigurationProvider : ConfigurationProvider
    {
        private readonly string _secretsPath;
        private readonly bool _optional;

        /// <summary>
        /// Creates provider instance that reads secrets from specified path.
        /// </summary>
        /// <param name="secretsPath">Path to folder containting secrets.</param>
        /// <param name="optional">Flag indicating wether secrets are optional or not.</param>
        public KeyVaultMountedVolumeConfigurationProvider(string secretsPath, bool optional)
        {
            if (secretsPath == null)
            {
                throw new ArgumentNullException(nameof(secretsPath));
            }
            _secretsPath = secretsPath;
            _optional = optional;
        }

        public override void Load()
        {
            try
            {
                Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var secretFilePath in Directory.EnumerateFiles(_secretsPath))
                {
                    var secret = File.ReadAllText(secretFilePath, Encoding.UTF8);
                    Data.Add(Path.GetFileNameWithoutExtension(secretFilePath), secret);
                }
            }
            catch (Exception e)
            {
                if (!_optional)
                {
                    throw new Exception($"Failed to read keyvault secrets from {_secretsPath}", e);
                }
            }
            OnReload();
        }
    }
}
