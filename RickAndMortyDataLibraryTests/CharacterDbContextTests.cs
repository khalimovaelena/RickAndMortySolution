using Xunit;
using Microsoft.EntityFrameworkCore;
using RickAndMortyDataLibrary.Models;
using RickAndMortyDataLibrary.Repository;
using System.Threading.Tasks;

public class CharacterDbContextTests
{
    private readonly DbContextOptions<CharacterDbContext> _dbContextOptions;

    public CharacterDbContextTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<CharacterDbContext>()
            .UseInMemoryDatabase(databaseName: "TestCharacterDB")
            .Options;
    }

    [Fact]
    public async Task Can_Add_And_Retrieve_Character()
    {
        using (var context = new CharacterDbContext(_dbContextOptions))
        {
            var character = new Character { Id = 1, Name = "Rick Sanchez", Status = "Alive", Species = "Human", Gender = "Male" };
            await context.Characters.AddAsync(character);
            await context.SaveChangesAsync();
        }

        using (var context = new CharacterDbContext(_dbContextOptions))
        {
            var retrievedCharacter = await context.Characters.FirstOrDefaultAsync(c => c.Id == 1);
            Assert.NotNull(retrievedCharacter);
            Assert.Equal("Rick Sanchez", retrievedCharacter.Name);
        }
    }
}