using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
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
        var service = _fixture.CreateService();
        Assert.NotNull(service);
    }

    [Fact]
    public async Task NavHistoryService_WithRealDI_LoadsJsonFromDisk()
    {
        // Arrange - Create a test JSON file on disk
        var testJson = @"{
            ""TESTFUND"": [
                { ""date"": ""2025-01-01"", ""navPrice"": 10.50, ""marketPrice"": 10.25 },
                { ""date"": ""2025-01-02"", ""navPrice"": 10.75, ""marketPrice"": 10.50 },
                { ""date"": ""2025-01-03"", ""navPrice"": 11.00, ""marketPrice"": 10.75 }
            ]
        }";

        var testDir = Path.Combine(Path.GetTempPath(), "navhistory-tests");
        Directory.CreateDirectory(testDir);
        var testFile = Path.Combine(testDir, "funds.json");

        try
        {
            await System.IO.File.WriteAllTextAsync(testFile, testJson);

            // Create a mock media item that returns the file path
            var mediaItem = Substitute.For<IMedia>();
            mediaItem.Name.Returns("funds.json");
            mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");
            mediaItem.GetValue<string>("path").Returns(testDir);

            _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });

            // Act - Use the actual NavHistoryService from DI
            var service = _fixture.CreateService();
            var result = await service.GetNavHistoryAsync("TESTFUND");

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
        finally
        {
            // Cleanup
            if (Directory.Exists(testDir))
            {
                try
                {
                    Directory.Delete(testDir, true);
                }
                catch { /* Ignore cleanup errors */ }
            }
        }
    }

    [Fact]
    public async Task NavHistoryService_WithNestedJson_LoadsFromDisk()
    {
        // Arrange
        var testJson = @"{
            ""TESTFUND"": {
                ""navHistory"": [
                    { ""date"": ""2025-01-01"", ""navPrice"": 10.50, ""marketPrice"": 10.25 },
                    { ""date"": ""2025-01-02"", ""navPrice"": 10.75, ""marketPrice"": 10.50 }
                ]
            }
        }";

        var testDir = Path.Combine(Path.GetTempPath(), "navhistory-tests-nested");
        Directory.CreateDirectory(testDir);
        var testFile = Path.Combine(testDir, "funds.json");

        try
        {
            await System.IO.File.WriteAllTextAsync(testFile, testJson);

            var mediaItem = Substitute.For<IMedia>();
            mediaItem.Name.Returns("funds.json");
            mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");
            mediaItem.GetValue<string>("path").Returns(testDir);

            _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });

            // Act
            var service = _fixture.CreateService();
            var result = await service.GetNavHistoryAsync("TESTFUND");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("TESTFUND", result[0].TickerCode);
            Assert.Equal(10.50m, result[0].NavPrice);
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                try
                {
                    Directory.Delete(testDir, true);
                }
                catch { /* Ignore */ }
            }
        }
    }

    [Fact]
    public async Task NavHistoryService_WithMissingMarketPrice_LoadsFromDisk()
    {
        // Arrange
        var testJson = @"{
            ""TESTFUND"": [
                { ""date"": ""2025-01-01"", ""navPrice"": 10.50 }
            ]
        }";

        var testDir = Path.Combine(Path.GetTempPath(), "navhistory-tests-optional");
        Directory.CreateDirectory(testDir);
        var testFile = Path.Combine(testDir, "funds.json");

        try
        {
            await System.IO.File.WriteAllTextAsync(testFile, testJson);

            var mediaItem = Substitute.For<IMedia>();
            mediaItem.Name.Returns("funds.json");
            mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");
            mediaItem.GetValue<string>("path").Returns(testDir);

            _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });

            // Act
            var service = _fixture.CreateService();
            var result = await service.GetNavHistoryAsync("TESTFUND");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(10.50m, result[0].NavPrice);
            Assert.Null(result[0].MarketPrice);
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                try
                {
                    Directory.Delete(testDir, true);
                }
                catch { /* Ignore */ }
            }
        }
    }

    [Fact]
    public async Task NavHistoryService_WithInvalidDate_HandlesGracefully()
    {
        // Arrange
        var testJson = @"{
            ""TESTFUND"": [
                { ""date"": ""invalid-date"", ""navPrice"": 10.50, ""marketPrice"": 10.25 }
            ]
        }";

        var testDir = Path.Combine(Path.GetTempPath(), "navhistory-tests-invalid");
        Directory.CreateDirectory(testDir);
        var testFile = Path.Combine(testDir, "funds.json");

        try
        {
            await System.IO.File.WriteAllTextAsync(testFile, testJson);

            var mediaItem = Substitute.For<IMedia>();
            mediaItem.Name.Returns("funds.json");
            mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");
            mediaItem.GetValue<string>("path").Returns(testDir);

            _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });

            // Act
            var service = _fixture.CreateService();
            var result = await service.GetNavHistoryAsync("TESTFUND");

            // Assert - Should still return a list with default date
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(default(DateTime), result[0].Date);
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                try
                {
                    Directory.Delete(testDir, true);
                }
                catch { /* Ignore */ }
            }
        }
    }

    [Fact]
    public async Task NavHistoryService_PreservesDataOrder()
    {
        // Arrange
        var testJson = @"{
            ""TESTFUND"": [
                { ""date"": ""2025-01-03"", ""navPrice"": 11.00 },
                { ""date"": ""2025-01-01"", ""navPrice"": 10.50 },
                { ""date"": ""2025-01-02"", ""navPrice"": 10.75 }
            ]
        }";

        var testDir = Path.Combine(Path.GetTempPath(), "navhistory-tests-order");
        Directory.CreateDirectory(testDir);
        var testFile = Path.Combine(testDir, "funds.json");

        try
        {
            await System.IO.File.WriteAllTextAsync(testFile, testJson);

            var mediaItem = Substitute.For<IMedia>();
            mediaItem.Name.Returns("funds.json");
            mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");
            mediaItem.GetValue<string>("path").Returns(testDir);

            _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });

            // Act
            var service = _fixture.CreateService();
            var result = await service.GetNavHistoryAsync("TESTFUND");

            // Assert - Verify list preserves JSON order
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(11.00m, result[0].NavPrice);
            Assert.Equal(10.50m, result[1].NavPrice);
            Assert.Equal(10.75m, result[2].NavPrice);
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                try
                {
                    Directory.Delete(testDir, true);
                }
                catch { /* Ignore */ }
            }
        }
    }

    [Fact]
    public async Task NavHistoryService_WithNullTicker_ReturnsNull()
    {
        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), "navhistory-tests-null");
        Directory.CreateDirectory(testDir);
        var testFile = Path.Combine(testDir, "funds.json");

        try
        {
            System.IO.File.WriteAllText(testFile, @"{ ""TEST"": [] }");

            var mediaItem = Substitute.For<IMedia>();
            mediaItem.Name.Returns("funds.json");
            mediaItem.GetValue<string>("umbracoFile").Returns("/media/funds.json");
            mediaItem.GetValue<string>("path").Returns(testDir.Replace(Directory.GetCurrentDirectory(), "").TrimStart('/'));

            _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });

            // Act
            var service = _fixture.CreateService();
            var result = await service.GetNavHistoryAsync(null);

            // Assert
            Assert.Null(result);
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                try
                {
                    Directory.Delete(testDir, true);
                }
                catch { /* Ignore */ }
            }
        }
    }
}

public class NavHistoryTestFixture
{
    public IMediaService MediaService { get; }
    public IServiceProvider ServiceProvider { get; }

    public NavHistoryTestFixture()
    {
        // Set up service collection with DI
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging();

        // Register the services (using NSubstitute for interfaces we can't instantiate)
        MediaService = Substitute.For<IMediaService>();

        services.AddSingleton(MediaService);
        services.AddScoped<INavHistoryService, NavHistoryService>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public INavHistoryService CreateService()
    {
        // Create a new scope for each test to get a fresh service instance
        var scope = ServiceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<INavHistoryService>();
    }
}
