using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RickAndMortyDataLibrary.Models
{
    public class ApiResponse
    {
        [JsonPropertyName("info")]
        public ApiInfo Info { get; set; }

        [JsonPropertyName("results")]
        public List<ApiCharacter> Results { get; set; }
    }

    public class ApiInfo
    {
        [JsonPropertyName("next")]
        public string Next { get; set; }
    }
}