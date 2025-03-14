using System;
using RickAndMortyDataLibrary.Models;

namespace RickAndMortyDataLibrary.Repository
{
	public interface ICharacterRepository
	{
        Task SaveCharactersAsync(IList<Character> characters);
        Task<DateTime?> GetLastUpdatedAsync();
        Task<IList<Character>> GetAllCharactersAsync();
        Task SaveCharacterAsync(Character character);
        Task<Character> GetCharacterByIdAsync(int id);
        Task<List<Character>> GetCharactersByPlanetAsync(string planetName);
    }
}

