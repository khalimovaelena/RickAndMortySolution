using Microsoft.Extensions.Logging;
using System.Text.Json;
using RickAndMortyDataLibrary.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace RickAndMortyDataLibrary.Services
{
    public class CharacterApiService: ICharacterApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CharacterApiService> _logger;
        private readonly string _apiUrl;

        public CharacterApiService(HttpClient httpClient,
            IConfiguration configuration,
            ILogger<CharacterApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _apiUrl = configuration["APIUrl"] ?? throw new ArgumentNullException(nameof(configuration), "APIUrl not found in configuration.");
        }

        public async Task<IList<Character>> FetchCharactersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching character data from API...");

                var allApiCharacters = await FetchAllCharactersFromApi();

                if (allApiCharacters.Any())
                {
                    var aliveCharacters = Map(allApiCharacters);
                    _logger.LogInformation($"Fetched {aliveCharacters.Count} characters");
                    return aliveCharacters;
                }
                else
                {
                    _logger.LogWarning("No characters received from API");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching characters: {ex.Message}");
            }

            return new List<Character>(); //if didn't fetche anything or error - return empty list
        }

        private static IList<Character> Map(IList<ApiCharacter> allApiCharacters)
        {
            // Map ApiCharacter to Character
            var aliveCharacters = allApiCharacters
                .Where(c => c.Status == "Alive")
                .Select(c => new Character
                {
                    Id = c.Id,
                    Name = c.Name,
                    Status = c.Status,
                    Species = c.Species,
                    Gender = c.Gender,
                    OriginPlanet = c.Origin?.Name, // Map to OriginPlanet
                    LocationPlanet = c.Location?.Name, // Map to LocationPlanet
                    LastUpdated = DateTime.UtcNow,
                })
                .ToList();

            return aliveCharacters;
        }

        private async Task<IList<ApiCharacter>> FetchAllCharactersFromApi()
        {
            var allApiCharacters = new List<ApiCharacter>();
            var nextPageUrl = _apiUrl;

            while (!string.IsNullOrEmpty(nextPageUrl))
            {
                var response = await _httpClient.GetStringAsync(nextPageUrl);
                var data = JsonSerializer.Deserialize<ApiResponse>(response);

                if (data?.Results != null)
                {
                    allApiCharacters.AddRange(data.Results);
                    nextPageUrl = data.Info?.Next; // Get next page URL
                }
                else
                {
                    break;
                }
            }

            return allApiCharacters;
        }
    }
}