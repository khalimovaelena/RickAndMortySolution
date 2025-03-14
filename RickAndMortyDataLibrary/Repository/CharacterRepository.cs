using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RickAndMortyDataLibrary.Models;

namespace RickAndMortyDataLibrary.Repository
{
	public class CharacterRepository: ICharacterRepository
    {
        private readonly CharacterDbContext _dbContext;
        private readonly ILogger<CharacterRepository> _logger;

        public CharacterRepository(CharacterDbContext dbContext, ILogger<CharacterRepository> logger)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _dbContext.Database.EnsureCreated();
            logger.LogInformation("Database is ready.");
        }

		public async Task SaveCharactersAsync(IList<Character> characters)
		{
			if (characters.Any())
			{
                try
                {
                    // Remove old characters
                    _dbContext.Characters.RemoveRange(_dbContext.Characters);

                    // Add characters to the database
                    await _dbContext.Characters.AddRangeAsync(characters);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Successfully stored {characters.Count} characters.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Couldn't store characters: {ex.Message}");
                }
            }
        }

        public async Task<DateTime?> GetLastUpdatedAsync()
        {
            return await Task.FromResult(_dbContext.Characters
                .OrderByDescending(c => c.LastUpdated)
                .Select(c => (DateTime?)c.LastUpdated)
                .FirstOrDefault());
        }

        public async Task<IList<Character>> GetAllCharactersAsync()
        {
            return await Task.FromResult(_dbContext.Characters.ToList());
        }

        public async Task SaveCharacterAsync(Character character)
        {
            await _dbContext.Characters.AddAsync(character);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Character> GetCharacterByIdAsync(int id)
        {
            return await Task.FromResult(_dbContext.Characters.FirstOrDefault(c => c.Id.Equals(id)));
        }

        public async Task<List<Character>> GetCharactersByPlanetAsync(string planetName)
        {
            return await _dbContext.Characters
                .Where(c => !string.IsNullOrEmpty(c.LocationPlanet) &&
                            EF.Functions.Like(c.LocationPlanet, $"%{planetName}%"))
                .ToListAsync();
        }
    }
}

