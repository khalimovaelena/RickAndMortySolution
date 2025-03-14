using System;
using RickAndMortyDataLibrary.Models;

namespace RickAndMortyDataLibrary.Services
{
	public interface ICharacterProvider
	{
        Task<CharacterProviderResult> GetCharactersAsync();
        Task AddCharacterAsync(Character character);
        Task<CharacterProviderResult> GetCharactersByPlanetAsync(string planetName);
        Task<CharacterProviderResult> GetCharacterByIdAsync(int id);
    }
}

