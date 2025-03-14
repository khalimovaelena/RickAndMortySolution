using Microsoft.EntityFrameworkCore;
using RickAndMortyDataLibrary.Models;

namespace RickAndMortyDataLibrary.Repository
{
    public class CharacterDbContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }

        public CharacterDbContext(DbContextOptions<CharacterDbContext> options)
            : base(options)
        {
        }
    }
}