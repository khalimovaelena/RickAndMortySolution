using System.Text.Json.Serialization;

namespace RickAndMortyDataLibrary.Models
{
    public class ApiCharacter
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("species")]
        public string? Species { get; set; }

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("origin")]
        public ApiPlanet Origin { get; set; }

        [JsonPropertyName("location")]
        public ApiPlanet Location { get; set; }
    }

    public class ApiPlanet
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
