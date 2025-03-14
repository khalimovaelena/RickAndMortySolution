using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RickAndMortyDataLibrary.Repository;
using RickAndMortyDataLibrary.Services;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())  // Ensure the base path is set correctly
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);  // Load appsettings.json

// Add services to the container.

// Use SQLite with the cross-platform path
builder.Services.AddDbContext<CharacterDbContext>(options =>
    options.UseSqlite($"Data Source={DatabasePathProvider.GetDatabasePath()}"));

// Register services
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddScoped<ICharacterProvider, CharacterProvider>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterApiService, CharacterApiService>();

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Enable routing to controllers
app.MapControllers();

app.Run();