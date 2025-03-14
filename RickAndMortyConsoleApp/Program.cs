using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RickAndMortyDataFetcher.Services;
using RickAndMortyDataLibrary.Repository;
using RickAndMortyDataLibrary.Services;

class Program
{
    static async Task Main()
    {
        // Create service provider with DI setup
        var serviceProvider = ServiceProviderFactory.Create();

        // Resolve required services
        var repository = serviceProvider.GetRequiredService<ICharacterRepository>();
        var apiService = serviceProvider.GetRequiredService<ICharacterApiService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Application started.");

        try
        {
            logger.LogInformation("Fetching and storing character data...");
            var apiCharacters = await apiService.FetchCharactersAsync();

            await repository.SaveCharactersAsync(apiCharacters);

            logger.LogInformation("Character data fetched and stored successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching or storing character data.");
        }

        logger.LogInformation("Application finished execution.");
    }
}