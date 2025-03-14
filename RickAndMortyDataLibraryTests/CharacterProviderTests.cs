using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RickAndMortyDataLibrary.Models;
using RickAndMortyDataLibrary.Services;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using RickAndMortyDataLibrary.Enums;
using RickAndMortyDataLibrary.Repository;

namespace RickAndMortyWebApp.Tests
{
    public class CharacterProviderTests
    {
        private readonly Mock<ICharacterRepository> _characterRepositoryMock;
        private readonly Mock<ICharacterApiService> _characterApiServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<CharacterProvider>> _loggerMock;
        private readonly CharacterProvider _characterProvider;

        public CharacterProviderTests()
        {
            _characterRepositoryMock = new Mock<ICharacterRepository>();
            _characterApiServiceMock = new Mock<ICharacterApiService>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<CharacterProvider>>();

            // Set up the configuration mock to return a value for DbExpirationMinutes
            _configurationMock.Setup(config => config["DbExpirationMinutes"]).Returns("10");

            _characterProvider = new CharacterProvider(
                _characterRepositoryMock.Object,
                _characterApiServiceMock.Object,
                _configurationMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetCharacterByIdAsync_ShouldReturnCharacterFromDbIfNotExpired()
        {
            // Arrange
            var characterId = 1;
            var character = new Character { Id = characterId, Name = "Rick Sanchez", Status = "Alive" };

            // Mock the repository to return a character when fetched by ID
            _characterRepositoryMock.Setup(repo => repo.GetCharacterByIdAsync(characterId))
                .ReturnsAsync(character);

            // Mock the last updated time to be within the expiration limit
            _characterRepositoryMock.Setup(repo => repo.GetLastUpdatedAsync())
                .ReturnsAsync(DateTime.UtcNow.AddMinutes(-5));  // Assume it's still valid in the DB

            // Act
            var result = await _characterProvider.GetCharacterByIdAsync(characterId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(characterId, result.Characters[0].Id);
            Assert.Equal("Rick Sanchez", result.Characters[0].Name);
            Assert.Equal(DataSources.Database, result.DataSource);
        }

        [Fact]
        public async Task GetCharacterByIdAsync_ShouldReturnCharacterFromApiIfDbExpired()
        {
            // Arrange
            var characterId = 1;
            var character = new Character { Id = characterId, Name = "Rick Sanchez", Status = "Alive" };
            var charactersFromApi = new List<Character> { character };

            // Mock the repository to return an expired last update time
            _characterRepositoryMock.Setup(repo => repo.GetLastUpdatedAsync())
                .ReturnsAsync(DateTime.UtcNow.AddMinutes(-15));  // Expired DB (assuming limit is 10 minutes)

            // Mock the API service to return the character
            _characterApiServiceMock.Setup(api => api.FetchCharactersAsync())
                .ReturnsAsync(charactersFromApi);

            // Mock saving fetched characters to the database
            _characterRepositoryMock.Setup(repo => repo.SaveCharactersAsync(It.IsAny<List<Character>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _characterProvider.GetCharacterByIdAsync(characterId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Characters);
            Assert.Equal(characterId, result.Characters[0].Id);
            Assert.Equal("Rick Sanchez", result.Characters[0].Name);
            Assert.Equal(DataSources.API, result.DataSource);  // Ensuring it came from API
        }
    }
}