# Rick and Morty Solution

## Overview
This project consists of a .NET 8 solution with 3 main components:
- the library with shared classes (such as Models, Repository to work with DB and Api Service to fetch data from external API)
- Console Application that uses library to fetch data from API and store it in DB: The data is sourced from the [Rick and Morty API](https://rickandmortyapi.com/api/character/) and is stored in an SQLite database.
- Web Application for displaying and managing characters.

## Project Structure
- **RickAndMortyDataLibrary**: shared library with classes to connect to DB and API.
- **RickAndMortyConsoleApp**: Fetches character data from the Rick and Morty API and stores only the characters with the status "Alive" in the SQLite database.
- **RickAndMortyWebApp**: Displays a list of characters, allows users to add new ones, and ensures retrieval happens at most once every five minutes unless new characters are added.
- **Unit Testing**: Includes unit tests to verify character retrieval and storage.

## Technologies Used
- .NET 8
- C#
- Entity Framework Core
- SQLite (lightweight local database, no SQL Server installation required)
- ASP.NET Core Web API
- Swagger to display data (works in every webbrowser, no UI setup required)
- xUnit (for unit testing)
- appsettings to store (and be able to easily change) external API url and database expiration time

## Setup and Installation
### Prerequisites
- .NET 8 SDK installed ([Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))
- No need to install SQL Server—SQLite is used as an embedded database.

## Features
### Console Application
- Fetches character data from the Rick and Morty API.
- Stores only characters with the status "Alive" in the SQLite database.
- Clears the database before saving new data.

### Web Application
- Retrieves and displays a list of characters.
- Allows adding new characters.
- Ensures characters are retrieved at most once every five minutes unless new data is added.
- Uses a response header `from-database` to indicate if the data was fetched from the database or API.
- Provides an endpoint to filter characters by planet (using EF.Like method)

### Unit Testing
- Tests for retrieving characters from the API and database.
- Verifies the expected number of characters returned.

## Future Enhancements
- Implement a frontend UI for better visualization.
- Add caching mechanisms for optimized API and DB calls.
- Deploy the application to a cloud environment.
- Add appsettings and new implementations of ICharacterRepository to be able to store data not only in local DB, but in cloud or any other type of DB/Storage
- Add more unit tests
- Implement integration tests for different types of DB/Storage