using System;
using System.IO;

namespace RickAndMortyDataLibrary.Services
{
    public static class DatabasePathProvider
    {
        public static string GetDatabasePath(string databaseName = "rickandmorty.db")
        {
            string databaseFolder;

            if (OperatingSystem.IsWindows())
            {
                databaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RickAndMortyApp");
            }
            else
            {
                // macOS/Linux: ~/.local/share/RickAndMortyApp
                databaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RickAndMortyApp");
            }

            // Ensure the folder exists
            if (!Directory.Exists(databaseFolder))
            {
                Directory.CreateDirectory(databaseFolder);
            }

            // Return full database path
            return Path.Combine(databaseFolder, databaseName);
        }
    }
}