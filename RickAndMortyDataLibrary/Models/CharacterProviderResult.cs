using System;
using RickAndMortyDataLibrary.Enums;

namespace RickAndMortyDataLibrary.Models
{
	public class CharacterProviderResult
	{
        public IList<Character> Characters { get; set; }
        public DataSources DataSource { get; set; } // "Database" or "API"

        public CharacterProviderResult(IList<Character> characters, DataSources dataSource)
        {
            Characters = characters;
            DataSource = dataSource;
        }
    }
}

