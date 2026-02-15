using Microsoft.Extensions.Logging;
using NSubstitute;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco13.Models;
using Umbraco13.Services;

namespace Umbraco13.Tests.Services;

public class NavHistoryServiceTests
{
    private readonly IMediaService _mediaService;
    private readonly IFileSystem _mediaFileSystem;
    private readonly ILogger<NavHistoryService> _logger;
    private readonly NavHistoryService _sut;

    public NavHistoryServiceTests()
    {
        _mediaService = Substitute.For<IMediaService>();
        _mediaFileSystem = Substitute.For<IFileSystem>();
        _logger = Substitute.For<ILogger<NavHistoryService>>();

        _sut = new NavHistoryService(_mediaService, _mediaFileSystem, _logger);
    }

    [Fact]
    public async Task GetNavHistoryAsync_WithNullTicker_ReturnsNull()
    {
        // Arrange
        string? nullTicker = null;

        // Act
        var result = await _sut.GetNavHistoryAsync(nullTicker!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetNavHistoryAsync_WhenFileNotFound_ReturnsNull()
    {
        // Arrange
        var ticker = "TEST";
        _mediaService.GetRootMedia().Returns(Array.Empty<IMedia>());

        // Act
        var result = await _sut.GetNavHistoryAsync(ticker);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetNavHistoryAsync_WithValidData_ReturnsListOfEntries()
    {
        // Arrange
        var ticker = "TESTFUND";
        var mediaItem = Substitute.For<IMedia>();
        mediaItem.Name.Returns("funds.json");
        mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");

        _mediaService.GetRootMedia().Returns(new[] { mediaItem });
        _mediaFileSystem.FileExists("/media/funds.json").Returns(true);

        var jsonContent = @"{
            ""TESTFUND"": [
                { ""date"": ""2025-01-01"", ""navPrice"": 10.50, ""marketPrice"": 10.25 },
                { ""date"": ""2025-01-02"", ""navPrice"": 10.75, ""marketPrice"": 10.50 }
            ]
        }";

        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
        _mediaFileSystem.OpenFile("/media/funds.json").Returns(stream);

        // Act
        var result = await _sut.GetNavHistoryAsync(ticker);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(ticker, result[0].TickerCode);
        Assert.Equal(10.50m, result[0].NavPrice);
        Assert.Equal(10.25m, result[0].MarketPrice);
    }

    [Fact]
    public async Task GetNavHistoryAsync_WithNestedNavHistory_ReturnsListOfEntries()
    {
        // Arrange
        var ticker = "TESTFUND";
        var mediaItem = Substitute.For<IMedia>();
        mediaItem.Name.Returns("funds.json");
        mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");

        _mediaService.GetRootMedia().Returns(new[] { mediaItem });
        _mediaFileSystem.FileExists("/media/funds.json").Returns(true);

        var jsonContent = @"{
            ""TESTFUND"": {
                ""navHistory"": [
                    { ""date"": ""2025-01-01"", ""navPrice"": 10.50, ""marketPrice"": 10.25 }
                ]
            }
        }";

        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
        _mediaFileSystem.OpenFile("/media/funds.json").Returns(stream);

        // Act
        var result = await _sut.GetNavHistoryAsync(ticker);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(10.50m, result[0].NavPrice);
    }

    [Fact]
    public async Task GetNavHistoryAsync_WhenTickerNotFound_ReturnsEmptyList()
    {
        // Arrange
        var ticker = "NONEXISTENT";
        var mediaItem = Substitute.For<IMedia>();
        mediaItem.Name.Returns("funds.json");
        mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");

        _mediaService.GetRootMedia().Returns(new[] { mediaItem });
        _mediaFileSystem.FileExists("/media/funds.json").Returns(true);

        var jsonContent = @"{
            ""OTHERFUND"": [
                { ""date"": ""2025-01-01"", ""navPrice"": 10.50 }
            ]
        }";

        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
        _mediaFileSystem.OpenFile("/media/funds.json").Returns(stream);

        // Act
        var result = await _sut.GetNavHistoryAsync(ticker);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetNavHistoryAsync_WithInvalidJson_ReturnsNull()
    {
        // Arrange
        var ticker = "TEST";
        var mediaItem = Substitute.For<IMedia>();
        mediaItem.Name.Returns("funds.json");
        mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");

        _mediaService.GetRootMedia().Returns(new[] { mediaItem });
        _mediaFileSystem.FileExists("/media/funds.json").Returns(true);

        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("{ invalid json }"));
        _mediaFileSystem.OpenFile("/media/funds.json").Returns(stream);

        // Act
        var result = await _sut.GetNavHistoryAsync(ticker);

        // Assert
        Assert.Null(result);
    }
}
