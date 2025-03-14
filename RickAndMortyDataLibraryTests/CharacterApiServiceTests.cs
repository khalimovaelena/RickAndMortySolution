using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using RickAndMortyDataLibrary.Models;
using RickAndMortyDataLibrary.Services;

public class CharacterApiServiceTests
{
    private readonly Mock<ILogger<CharacterApiService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly HttpClient _httpClient;
    private readonly CharacterApiService _apiService;

    public CharacterApiServiceTests()
    {
        _loggerMock = new Mock<ILogger<CharacterApiService>>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["APIUrl"]).Returns("https://test.api.com/api/character");

        _httpClient = new HttpClient(new MockHttpMessageHandler()); // MockHttpMessageHandler to simulate API calls
        _apiService = new CharacterApiService(_httpClient, _configurationMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task FetchCharactersAsync_ShouldReturnAliveCharacters()
    {
        var result = await _apiService.FetchCharactersAsync();
        Assert.NotEmpty(result);
        Assert.All(result, c => Assert.Equal("Alive", c.Status));
    }
}