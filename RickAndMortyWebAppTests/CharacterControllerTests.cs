using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RickAndMortyDataLibrary.Models;
using RickAndMortyDataLibrary.Services;
using RickAndMortyDataLibrary.Enums;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RickAndMortyWebApp.Controllers;
using Microsoft.AspNetCore.Http;

namespace RickAndMortyWebApp.Tests
{
    public class CharacterControllerTests
    {
        private readonly Mock<ICharacterProvider> _characterProviderMock;
        private readonly Mock<ILogger<CharacterController>> _loggerMock;
        private readonly CharacterController _controller;
        private readonly DefaultHttpContext _httpContext;

        public CharacterControllerTests()
        {
            _characterProviderMock = new Mock<ICharacterProvider>();
            _loggerMock = new Mock<ILogger<CharacterController>>();

            _httpContext = new DefaultHttpContext(); // Create a mock HttpContext
            _controller = new CharacterController(_characterProviderMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };
        }

        [Fact]
        public async Task GetCharacters_ReturnsOkResult_WithCharacters()
        {
            // Arrange
            var characters = new List<Character>
            {
                new Character { Id = 1, Name = "Rick Sanchez", Status = "Alive" },
                new Character { Id = 2, Name = "Morty Smith", Status = "Alive" }
            };

            var result = new CharacterProviderResult(characters, DataSources.API);
            _characterProviderMock.Setup(cp => cp.GetCharactersAsync()).ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetCharacters();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnValue = Assert.IsAssignableFrom<List<Character>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);

            // Assert if headers are added correctly
            Assert.Contains("from-database", _httpContext.Response.Headers.Keys);
            Assert.Equal("false", _httpContext.Response.Headers["from-database"]);
        }

        [Fact]
        public async Task AddCharacter_ReturnsCreatedAtActionResult_WhenCharacterIsValid()
        {
            // Arrange
            var character = new Character { Id = 1, Name = "Rick Sanchez", Status = "Alive" };
            _characterProviderMock.Setup(cp => cp.AddCharacterAsync(character)).Returns(Task.CompletedTask);

            // Act
            var actionResult = await _controller.AddCharacter(character);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult);
            Assert.Equal("GetCharacterById", createdAtActionResult.ActionName);
            Assert.Equal(character.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(character, createdAtActionResult.Value);
        }

        [Fact]
        public async Task AddCharacter_ReturnsBadRequest_WhenCharacterIsNull()
        {
            // Act
            var actionResult = await _controller.AddCharacter(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("Character is required", badRequestResult.Value);
        }

        [Fact]
        public async Task GetCharactersByPlanet_ReturnsOkResult_WhenCharactersFound()
        {
            // Arrange
            var characters = new List<Character>
            {
                new Character { Id = 1, Name = "Rick Sanchez", Status = "Alive", LocationPlanet = "Earth" }
            };

            var result = new CharacterProviderResult(characters, DataSources.API);
            _characterProviderMock.Setup(cp => cp.GetCharactersByPlanetAsync("Earth")).ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetCharactersByPlanet("Earth");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnValue = Assert.IsAssignableFrom<List<Character>>(okResult.Value);
            Assert.Equal(1, returnValue.Count);
        }

        [Fact]
        public async Task GetCharactersByPlanet_ReturnsNotFound_WhenNoCharactersFound()
        {
            // Arrange
            var result = new CharacterProviderResult(new List<Character>(), DataSources.API);
            _characterProviderMock.Setup(cp => cp.GetCharactersByPlanetAsync("Mars")).ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetCharactersByPlanet("Mars");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            Assert.Equal("No characters found for planet Mars", notFoundResult.Value);
        }

        [Fact]
        public async Task GetCharacterById_ReturnsOkResult_WhenCharacterFound()
        {
            // Arrange
            var character = new Character { Id = 1, Name = "Rick Sanchez", Status = "Alive" };
            var result = new CharacterProviderResult(new List<Character> { character }, DataSources.API);
            _characterProviderMock.Setup(cp => cp.GetCharacterByIdAsync(1)).ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetCharacterById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnValue = Assert.IsType<Character>(okResult.Value);
            Assert.Equal(character.Id, returnValue.Id);
            Assert.Equal(character.Name, returnValue.Name);
        }

        [Fact]
        public async Task GetCharacterById_ReturnsNotFound_WhenCharacterNotFound()
        {
            // Arrange
            var result = new CharacterProviderResult(new List<Character>(), DataSources.API);
            _characterProviderMock.Setup(cp => cp.GetCharacterByIdAsync(1)).ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetCharacterById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            Assert.Equal("Character with ID 1 not found", notFoundResult.Value);
        }
    }
}