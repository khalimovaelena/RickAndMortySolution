using System;
using System.Text.Json.Serialization;

namespace RickAndMortyDataLibrary.Models
{
    public class Character
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Status { get; set; }

            public string? Species { get; set; }

            public string? Gender { get; set; }

            // Store the name of the origin planet as a string
            public string? OriginPlanet { get; set; }

            // Store the name of the current location planet as a string
            public string? LocationPlanet { get; set; }

            public DateTime LastUpdated { get; set; }
    }
}