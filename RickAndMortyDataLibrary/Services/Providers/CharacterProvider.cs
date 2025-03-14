using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RickAndMortyDataLibrary.Enums;
using RickAndMortyDataLibrary.Models;
using RickAndMortyDataLibrary.Repository;

namespace RickAndMortyDataLibrary.Services
{
	public class CharacterProvider: ICharacterProvider
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ICharacterApiService _characterApiService;
        private readonly int _dbExpirationMinutes;
        private readonly ILogger<CharacterProvider> _logger;

        public CharacterProvider(
            ICharacterRepository characterRepository,
            ICharacterApiService characterApiService,
            IConfiguration configuration,
            ILogger<CharacterProvider> logger)
        {
            _characterRepository = characterRepository;
            _characterApiService = characterApiService;
            _logger = logger;
            int.TryParse(configuration["DbExpirationMinutes"], out _dbExpirationMinutes);
        }

        public async Task AddCharacterAsync(Character character)
        {
            character.LastUpdated = DateTime.UtcNow;
            await _characterRepository.SaveCharacterAsync(character);
        }

        public async Task<CharacterProviderResult> GetCharacterByIdAsync(int id)
        {
            if (await ShouldExtractFromDb())
            {
                _logger.LogInformation("Fetching character from database.");
                var characterFromDb = await _characterRepository.GetCharacterByIdAsync(id);
                var charactersList = new List<Character>
                {
                    characterFromDb
                };

                return new CharacterProviderResult(charactersList, DataSources.Database);
            }
            else
            {
                _logger.LogInformation("Fetching characters from API.");
                var fetchedCharacters = await _characterApiService.FetchCharactersAsync();

                await _characterRepository.SaveCharactersAsync(fetchedCharacters);

                var character = fetchedCharacters.FirstOrDefault(c => c.Id.Equals(id));
                var charactersList = new List<Character>
                {
                    character
                };

                return new CharacterProviderResult(charactersList, DataSources.API);
            }
        }

        public async Task<CharacterProviderResult> GetCharactersAsync()
        {
            if (await ShouldExtractFromDb())
            {
                _logger.LogInformation("Fetching characters from database.");
                var charactersFromDb = await _characterRepository.GetAllCharactersAsync();
                return new CharacterProviderResult(charactersFromDb, DataSources.Database);
            }
            else
            {
                _logger.LogInformation("Fetching characters from API.");
                var fetchedCharacters = await _characterApiService.FetchCharactersAsync();

                await _characterRepository.SaveCharactersAsync(fetchedCharacters);
                return new CharacterProviderResult(fetchedCharacters, DataSources.API);
            }
        }

        public async Task<CharacterProviderResult> GetCharactersByPlanetAsync(string planetName)
        {
            if (await ShouldExtractFromDb())
            {
                _logger.LogInformation("Fetching characters from database.");
                var charactersFromDb = await _characterRepository.GetCharactersByPlanetAsync(planetName);
                return new CharacterProviderResult(charactersFromDb, DataSources.Database);
            }
            else
            {
                _logger.LogInformation("Fetching characters from API.");
                var fetchedCharacters = await _characterApiService.FetchCharactersAsync();
                await _characterRepository.SaveCharactersAsync(fetchedCharacters);

                var filteredCharacters = fetchedCharacters
                    .Where(c => !string.IsNullOrEmpty(c.LocationPlanet) &&
                                c.LocationPlanet.Contains(planetName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return new CharacterProviderResult(filteredCharacters, DataSources.API);
            }
        }

        private async Task<bool> ShouldExtractFromDb()
        {
            var lastUpdated = await _characterRepository.GetLastUpdatedAsync();
            var timeSinceLastUpdate = lastUpdated.HasValue
                ? (DateTime.UtcNow - lastUpdated.Value).TotalMinutes
                : double.MaxValue;
            return lastUpdated.HasValue && timeSinceLastUpdate <= _dbExpirationMinutes;
        }
    }
}

