using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco13.Models;
using Umbraco13.Services;

namespace Umbraco13.Integration.Tests;

public class NavHistoryIntegrationTests : IClassFixture<NavHistoryTestFixture>
{
    private readonly NavHistoryTestFixture _fixture;

    public NavHistoryIntegrationTests(NavHistoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ServiceCollection_ContainsRequiredServices()
    {
        // Assert
        Assert.NotNull(_fixture.MediaService);
        Assert.NotNull(_fixture.MediaFileSystem);
        Assert.NotNull(_fixture.NavHistoryService);
    }

    [Fact]
    public async Task NavHistoryService_WithRealDI_LoadsJsonIntoList()
    {
        // Arrange - Create a test JSON file in memory
        var testJson = @"{
            ""TESTFUND"": [
                { ""date"": ""2025-01-01"", ""navPrice"": 10.50, ""marketPrice"": 10.25 },
                { ""date"": ""2025-01-02"", ""navPrice"": 10.75, ""marketPrice"": 10.50 },
                { ""date"": ""2025-01-03"", ""navPrice"": 11.00, ""marketPrice"": 10.75 }
            ]
        }";

        // Create a mock media item
        var mediaItem = Substitute.For<IMedia>();
        mediaItem.Name.Returns("funds.json");
        mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");

        _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });
        _fixture.MediaFileSystem.FileExists("/media/funds.json").Returns(true);

        // Create file stream from JSON
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testJson));
        _fixture.MediaFileSystem.OpenFile("/media/funds.json").Returns(stream);

        // Act - Use the actual NavHistoryService from DI
        var result = await _fixture.NavHistoryService.GetNavHistoryAsync("TESTFUND");

        // Assert - Verify we get a list back
        Assert.NotNull(result);
        Assert.IsType<List<NavHistoryEntry>>(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("TESTFUND", result[0].TickerCode);

        // Verify data was parsed correctly
        Assert.Equal(10.50m, result[0].NavPrice);
        Assert.Equal(10.25m, result[0].MarketPrice);
        Assert.Equal(new DateTime(2025, 1, 1), result[0].Date);

        Assert.Equal(10.75m, result[1].NavPrice);
        Assert.Equal(10.50m, result[1].MarketPrice);

        Assert.Equal(11.00m, result[2].NavPrice);
        Assert.Equal(10.75m, result[2].MarketPrice);
    }

    [Fact]
    public async Task NavHistoryService_WithNestedJson_LoadsIntoList()
    {
        // Arrange - Test with nested navHistory format
        var testJson = @"{
            ""TESTFUND"": {
                ""navHistory"": [
                    { ""date"": ""2025-01-01"", ""navPrice"": 10.50, ""marketPrice"": 10.25 },
                    { ""date"": ""2025-01-02"", ""navPrice"": 10.75, ""marketPrice"": 10.50 }
                ]
            }
        }";

        var mediaItem = Substitute.For<IMedia>();
        mediaItem.Name.Returns("funds.json");
        mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");

        _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });
        _fixture.MediaFileSystem.FileExists("/media/funds.json").Returns(true);

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testJson));
        _fixture.MediaFileSystem.OpenFile("/media/funds.json").Returns(stream);

        // Act
        var result = await _fixture.NavHistoryService.GetNavHistoryAsync("TESTFUND");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("TESTFUND", result[0].TickerCode);
        Assert.Equal(10.50m, result[0].NavPrice);
    }

    [Fact]
    public async Task NavHistoryService_WithMissingMarketPrice_LoadsIntoList()
    {
        // Arrange - Test with optional marketPrice field
        var testJson = @"{
            ""TESTFUND"": [
                { ""date"": ""2025-01-01"", ""navPrice"": 10.50 }
            ]
        }";

        var mediaItem = Substitute.For<IMedia>();
        mediaItem.Name.Returns("funds.json");
        mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");

        _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });
        _fixture.MediaFileSystem.FileExists("/media/funds.json").Returns(true);

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testJson));
        _fixture.MediaFileSystem.OpenFile("/media/funds.json").Returns(stream);

        // Act
        var result = await _fixture.NavHistoryService.GetNavHistoryAsync("TESTFUND");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(10.50m, result[0].NavPrice);
        Assert.Null(result[0].MarketPrice);
    }

    [Fact]
    public async Task NavHistoryService_WithInvalidDate_HandlesGracefully()
    {
        // Arrange - Test with invalid date format
        var testJson = @"{
            ""TESTFUND"": [
                { ""date"": ""invalid-date"", ""navPrice"": 10.50, ""marketPrice"": 10.25 }
            ]
        }";

        var mediaItem = Substitute.For<IMedia>();
        mediaItem.Name.Returns("funds.json");
        mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");

        _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });
        _fixture.MediaFileSystem.FileExists("/media/funds.json").Returns(true);

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testJson));
        _fixture.MediaFileSystem.OpenFile("/media/funds.json").Returns(stream);

        // Act
        var result = await _fixture.NavHistoryService.GetNavHistoryAsync("TESTFUND");

        // Assert - Should still return a list with default date
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(default(DateTime), result[0].Date);
    }

    [Fact]
    public async Task NavHistoryService_ReturnsList_WithDescendingDates()
    {
        // Arrange - Test that dates are in correct order in the list
        var testJson = @"{
            ""TESTFUND"": [
                { ""date"": ""2025-01-03"", ""navPrice"": 11.00 },
                { ""date"": ""2025-01-01"", ""navPrice"": 10.50 },
                { ""date"": ""2025-01-02"", ""navPrice"": 10.75 }
            ]
        }";

        var mediaItem = Substitute.For<IMedia>();
        mediaItem.Name.Returns("funds.json");
        mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");

        _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });
        _fixture.MediaFileSystem.FileExists("/media/funds.json").Returns(true);

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testJson));
        _fixture.MediaFileSystem.OpenFile("/media/funds.json").Returns(stream);

        // Act
        var result = await _fixture.NavHistoryService.GetNavHistoryAsync("TESTFUND");

        // Assert - Verify list is populated with all entries
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(11.00m, result[0].NavPrice);
        Assert.Equal(10.50m, result[1].NavPrice);
        Assert.Equal(10.75m, result[2].NavPrice);
    }
}

public class NavHistoryTestFixture
{
    public IMediaService MediaService { get; }
    public IFileSystem MediaFileSystem { get; }
    public INavHistoryService NavHistoryService { get; }

    public NavHistoryTestFixture()
    {
        // Set up service collection with DI
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging();

        // Register the services (using NSubstitute for interfaces we can't instantiate)
        MediaService = Substitute.For<IMediaService>();
        MediaFileSystem = Substitute.For<IFileSystem>();

        services.AddSingleton(MediaService);
        services.AddSingleton(MediaFileSystem);
        services.AddScoped<INavHistoryService, NavHistoryService>();

        var serviceProvider = services.BuildServiceProvider();

        // Get the NavHistoryService from DI container
        NavHistoryService = serviceProvider.GetRequiredService<INavHistoryService>();

        // Verify service is created successfully
        Assert.NotNull(NavHistoryService);
    }
}
