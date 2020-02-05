using AuctionHouse.Common.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddCommonSecrets(this IConfigurationBuilder builder, string secretsPath, bool optional = false)
        {
            builder.Add(new KeyVaultMountedVolumeConfigurationSource(secretsPath, optional));
            return builder;
        }
    }
}
