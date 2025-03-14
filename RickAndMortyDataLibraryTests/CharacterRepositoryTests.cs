using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RickAndMortyDataLibrary.Models;
using RickAndMortyDataLibrary.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RickAndMortyDataLibraryTests
{
    public class CharacterRepositoryTests: IDisposable
    {
        private readonly Mock<ILogger<CharacterRepository>> _loggerMock;
        private CharacterRepository _characterRepository;
        private DbContextOptions<CharacterDbContext> _dbContextOptions;
        private CharacterDbContext _context;

        public CharacterRepositoryTests()
        {
            _loggerMock = new Mock<ILogger<CharacterRepository>>();
        }

        // Set up an in-memory DbContext for testing
        private void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<CharacterDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new CharacterDbContext(_dbContextOptions);

            // Add sample data to the in-memory database
            _context.Characters.Add(new Character { Id = 1, Name = "Rick Sanchez", Status = "Alive" });
            _context.Characters.Add(new Character { Id = 2, Name = "Morty Smith", Status = "Dead" });
            _context.SaveChanges();

            // Create the repository with the in-memory DbContext
            _characterRepository = new CharacterRepository(_context, _loggerMock.Object);
        }

        // Test for getting a character by ID
        [Fact]
        public async Task GetCharacterByIdAsync_ShouldReturnCharacter()
        {
            // Arrange: Set up a fresh DbContext for this test
            Setup();

            // Act: Fetch the character by ID
            var result = await _characterRepository.GetCharacterByIdAsync(1);

            // Assert: Verify the character is retrieved correctly
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Rick Sanchez", result.Name);
        }

        // Test for saving a new character
        [Fact]
        public async Task SaveCharacterAsync_ShouldSaveCharacter()
        {
            // Arrange: Set up a fresh DbContext for this test
            Setup();

            // Create a new character
            var newCharacter = new Character { Id = 3, Name = "Beth Smith", Status = "Alive" };

            // Act: Save the character
            await _characterRepository.SaveCharacterAsync(newCharacter);

            // Assert: Verify the character is saved correctly
            var savedCharacter = await _characterRepository.GetCharacterByIdAsync(3);
            Assert.NotNull(savedCharacter);
            Assert.Equal("Beth Smith", savedCharacter.Name);
        }

        // Test for saving multiple characters
        [Fact]
        public async Task SaveCharactersAsync_ShouldSaveMultipleCharacters()
        {
            // Arrange: Set up a fresh DbContext for this test
            Setup();

            // Create a list of characters
            var characters = new List<Character>
            {
                new Character { Id = 4, Name = "Summer Smith", Status = "Alive" },
                new Character { Id = 5, Name = "Jerry Smith", Status = "Alive" }
            };

            // Act: Save the list of characters
            await _characterRepository.SaveCharactersAsync(characters);

            // Assert: Verify the characters are saved correctly
            var savedCharacter1 = await _characterRepository.GetCharacterByIdAsync(4);
            Assert.NotNull(savedCharacter1);
            Assert.Equal("Summer Smith", savedCharacter1.Name);

            var savedCharacter2 = await _characterRepository.GetCharacterByIdAsync(5);
            Assert.NotNull(savedCharacter2);
            Assert.Equal("Jerry Smith", savedCharacter2.Name);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Cleanup the database after tests
            _context.Dispose();
        }
    }
}