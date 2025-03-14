using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RickAndMortyDataLibrary.Repository;
using RickAndMortyDataLibrary.Services;

namespace RickAndMortyDataFetcher.Services
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider Create()
        {
            var services = new ServiceCollection();

            //TODO: use appsettings to determine the place where DB is stored: locally or in AWS or in any other place
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Configure logging to console
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information); // Adjust log level if necessary
            });

            services.AddDbContext<CharacterDbContext>(options =>
                options.UseSqlite($"Data Source={DatabasePathProvider.GetDatabasePath()}"),
                ServiceLifetime.Singleton);

            services.AddSingleton<HttpClient>();
            services.AddScoped<ICharacterProvider, CharacterProvider>();
            services.AddScoped<ICharacterRepository, CharacterRepository>();
            services.AddScoped<ICharacterApiService, CharacterApiService>();

            return services.BuildServiceProvider();
        }
    }
}