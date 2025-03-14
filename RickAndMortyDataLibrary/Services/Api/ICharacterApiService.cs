using RickAndMortyDataLibrary.Models;

namespace RickAndMortyDataLibrary.Services
{
	public interface ICharacterApiService
	{
        Task<IList<Character>> FetchCharactersAsync();
    }
}

