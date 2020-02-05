using AuctionHouse.Common.Interfaces;
using AuctionHouse.Common.Services;
using AuctionHouse.SnapshotService.Extensions;
using AuctionHouse.SnapshotService.Models;
using AuctionHouse.SnapshotService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace AuctionHouse.SnapshotService
{
    public class Startup
    {
        private readonly IConfigurationRoot _config;
        private readonly IHostEnvironment _env;

        public Startup(IHostEnvironment environment)
        {
            _env = environment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddEnvironmentVariables()
                .AddServiceConfigurations();

            _config = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<HttpClient>();
            services.AddSingleton<IAuthService, AuthService>(sp =>
            {
                var authUrl = _config[Common.Constants.ConfigurationKeys.AuthTokenBaseUrl];
                var clientId = _config[Common.Constants.ConfigurationKeys.AuthClientId];
                var clientSecret = _config[Common.Constants.ConfigurationKeys.AuthClientSecret];
                return new AuthService(sp.GetService<HttpClient>(), authUrl, clientId, clientSecret);
            });
            services.AddSingleton<ITokenHandler, TokenHandler>();
            services.AddTransient<IAuctionHouseService, AuctionHouseService>(sp => new AuctionHouseService(sp.GetService<HttpClient>(), sp.GetService<ITokenHandler>(), _config[Common.Constants.ConfigurationKeys.ApiBaseUrl]));
            services.AddTransient(sp => new SnapshotDbContext(_config[Constants.ConfigurationKeys.DbConnectionString]));
            services.AddSingleton<IEnumerable<Common.Models.Realm>>(sp =>
            {
                var realmsJson = _config[Constants.ConfigurationKeys.DefaultRealmsJson];
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Common.Models.Realm>>(realmsJson);
            });
            services.AddHostedService<SnapshotFetcher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class MountedKeyVaultConfigurationProvider : ConfigurationProvider
    {
        private readonly string _secretsPath;

        public MountedKeyVaultConfigurationProvider(string secretsPath)
        {
            if (string.IsNullOrEmpty(secretsPath)) throw new ArgumentNullException(nameof(secretsPath));
            _secretsPath = secretsPath;
        }

        public override void Load()
        {
            Data = new Dictionary<string, string>();
            var secretFiles = System.IO.Directory.GetFiles(_secretsPath);
            foreach (var file in secretFiles)
            {
                var contents = System.IO.File.ReadAllText(file);
                Data.Add(Path.GetFileNameWithoutExtension(file), contents);
            }

            OnReload();
        }
    }
}
