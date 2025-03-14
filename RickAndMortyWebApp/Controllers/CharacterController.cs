using Microsoft.AspNetCore.Mvc;
using RickAndMortyDataLibrary.Enums;
using RickAndMortyDataLibrary.Models;
using RickAndMortyDataLibrary.Services;

namespace RickAndMortyWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterProvider _characterProvider;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(ICharacterProvider characterProvider, ILogger<CharacterController> logger)
        {
            _characterProvider = characterProvider ?? throw new ArgumentNullException(nameof(characterProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/character
        [HttpGet]
        public async Task<IActionResult> GetCharacters()
        {
            var result = await _characterProvider.GetCharactersAsync();
            Response.Headers.Add("from-database", result.DataSource == DataSources.Database ? "true" : "false");
            return Ok(result.Characters);
        }

        // POST: api/character
        [HttpPost]
        public async Task<IActionResult> AddCharacter([FromBody] Character character)
        {
            if (character == null)
            {
                return BadRequest("Character is required");
            }

            await _characterProvider.AddCharacterAsync(character);
            return CreatedAtAction(nameof(GetCharacterById), new { id = character.Id }, character);
        }

        // GET: api/character/planet/{planetName}
        [HttpGet("planet/{planetName}")]
        public async Task<IActionResult> GetCharactersByPlanet(string planetName)
        {
            var result = await _characterProvider.GetCharactersByPlanetAsync(planetName);
            Response.Headers.Add("from-database", result.DataSource == DataSources.Database ? "true" : "false");
            return result.Characters.Any() ? Ok(result.Characters) : NotFound($"No characters found for planet {planetName}");
        }

        // GET: api/character/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCharacterById(int id)
        {
            var result = await _characterProvider.GetCharacterByIdAsync(id);

            Response.Headers.Add("from-database", result.DataSource == DataSources.Database ? "true" : "false");

            var character = result.Characters.FirstOrDefault();

            if (character == null)
            {
                return NotFound($"Character with ID {id} not found");
            }

            return Ok(character);
        }
    }
}