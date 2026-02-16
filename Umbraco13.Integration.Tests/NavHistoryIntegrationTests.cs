using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.IO;
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
            ""funds"": [
                {
                    ""id"": 1,
                    ""fundName"": ""Test Fund"",
                    ""tickerCode"": ""TESTFUND"",
                    ""navPrice"": 10.50,
                    ""marketPrice"": 10.25,
                    ""holdInTrust"": ""Yes"",
                    ""historicalNav"": [
                        {
                            ""id"": 1,
                            ""fundId"": 1,
                            ""navPrice"": 10.50,
                            ""marketPrice"": 10.25,
                            ""navDate"": ""2025-01-01T00:00:00"",
                            ""dailyChangePercent"": 0.5,
                            ""netAssetValue"": 1000000
                        },
                        {
                            ""id"": 2,
                            ""fundId"": 1,
                            ""navPrice"": 10.75,
                            ""marketPrice"": 10.50,
                            ""navDate"": ""2025-01-02T00:00:00"",
                            ""dailyChangePercent"": 0.6,
                            ""netAssetValue"": 1000000
                        },
                        {
                            ""id"": 3,
                            ""fundId"": 1,
                            ""navPrice"": 11.00,
                            ""marketPrice"": 10.75,
                            ""navDate"": ""2025-01-03T00:00:00"",
                            ""dailyChangePercent"": 0.7,
                            ""netAssetValue"": 1000000
                        }
                    ]
                }
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
            mediaItem.GetValue<string>("umbracoFile").Returns(testFile);

            _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });

            // Act - Use the actual NavHistoryService from DI
            var service = _fixture.CreateService();
            var result = await service.GetNavHistoryAsync("TESTFUND");

            // Assert - Verify we get a list back
            Assert.NotNull(result);
            Assert.IsType<List<NavHistoryEntry>>(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("TESTFUND", result[0].TickerCode);

            // Verify data was parsed correctly (ordered by date descending)
            Assert.Equal(11.00m, result[0].NavPrice);
            Assert.Equal(10.75m, result[0].MarketPrice);
            Assert.Equal(new DateTime(2025, 1, 3), result[0].Date);

            Assert.Equal(10.75m, result[1].NavPrice);
            Assert.Equal(10.50m, result[1].MarketPrice);

            Assert.Equal(10.50m, result[2].NavPrice);
            Assert.Equal(10.25m, result[2].MarketPrice);
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
            ""funds"": [
                {
                    ""id"": 1,
                    ""fundName"": ""Test Fund"",
                    ""tickerCode"": ""TESTFUND"",
                    ""navPrice"": 10.50,
                    ""marketPrice"": 10.25,
                    ""holdInTrust"": ""Yes"",
                    ""historicalNav"": [
                        {
                            ""id"": 1,
                            ""fundId"": 1,
                            ""navPrice"": 10.50,
                            ""marketPrice"": 10.25,
                            ""navDate"": ""2025-01-01T00:00:00"",
                            ""dailyChangePercent"": 0.5,
                            ""netAssetValue"": 1000000
                        },
                        {
                            ""id"": 2,
                            ""fundId"": 1,
                            ""navPrice"": 10.75,
                            ""marketPrice"": 10.50,
                            ""navDate"": ""2025-01-02T00:00:00"",
                            ""dailyChangePercent"": 0.6,
                            ""netAssetValue"": 1000000
                        }
                    ]
                }
            ]
        }";

        var testDir = Path.Combine(Path.GetTempPath(), "navhistory-tests-nested");
        Directory.CreateDirectory(testDir);
        var testFile = Path.Combine(testDir, "funds.json");

        try
        {
            await System.IO.File.WriteAllTextAsync(testFile, testJson);

            var mediaItem = Substitute.For<IMedia>();
            mediaItem.Name.Returns("funds.json");
            mediaItem.GetValue<string>("umbracoFile").Returns(testFile);

            _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });

            // Act
            var service = _fixture.CreateService();
            var result = await service.GetNavHistoryAsync("TESTFUND");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("TESTFUND", result[0].TickerCode);
            Assert.Equal(10.75m, result[0].NavPrice); // Most recent (2025-01-02)
            Assert.Equal(10.50m, result[1].NavPrice); // Older (2025-01-01)
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
            ""funds"": [
                {
                    ""id"": 1,
                    ""fundName"": ""Test Fund"",
                    ""tickerCode"": ""TESTFUND"",
                    ""navPrice"": 10.50,
                    ""marketPrice"": 10.25,
                    ""holdInTrust"": ""Yes"",
                    ""historicalNav"": [
                        {
                            ""id"": 1,
                            ""fundId"": 1,
                            ""navPrice"": 10.50,
                            ""marketPrice"": null,
                            ""navDate"": ""2025-01-01T00:00:00"",
                            ""dailyChangePercent"": 0.5,
                            ""netAssetValue"": 1000000
                        }
                    ]
                }
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
            mediaItem.GetValue<string>("umbracoFile").Returns(testFile);

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
    public async Task NavHistoryService_WithInvalidDate_FailsGracefully()
    {
        // Arrange
        var testJson = @"{
            ""funds"": [
                {
                    ""id"": 1,
                    ""fundName"": ""Test Fund"",
                    ""tickerCode"": ""TESTFUND"",
                    ""navPrice"": 10.50,
                    ""marketPrice"": 10.25,
                    ""holdInTrust"": ""Yes"",
                    ""historicalNav"": [
                        {
                            ""id"": 1,
                            ""fundId"": 1,
                            ""navPrice"": 10.50,
                            ""marketPrice"": 10.25,
                            ""navDate"": ""invalid-date"",
                            ""dailyChangePercent"": 0.5,
                            ""netAssetValue"": 1000000
                        }
                    ]
                }
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
            mediaItem.GetValue<string>("umbracoFile").Returns(testFile);

            _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });

            // Act
            var service = _fixture.CreateService();
            var result = await service.GetNavHistoryAsync("TESTFUND");

            // Assert - Invalid JSON should cause deserialization to fail, returning empty result
            Assert.NotNull(result);
            Assert.Empty(result);
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
            ""funds"": [
                {
                    ""id"": 1,
                    ""fundName"": ""Test Fund"",
                    ""tickerCode"": ""TESTFUND"",
                    ""navPrice"": 10.50,
                    ""marketPrice"": 10.25,
                    ""holdInTrust"": ""Yes"",
                    ""historicalNav"": [
                        {
                            ""id"": 1,
                            ""fundId"": 1,
                            ""navPrice"": 11.00,
                            ""marketPrice"": 10.75,
                            ""navDate"": ""2025-01-03T00:00:00"",
                            ""dailyChangePercent"": 0.7,
                            ""netAssetValue"": 1000000
                        },
                        {
                            ""id"": 2,
                            ""fundId"": 1,
                            ""navPrice"": 10.50,
                            ""marketPrice"": 10.25,
                            ""navDate"": ""2025-01-01T00:00:00"",
                            ""dailyChangePercent"": 0.5,
                            ""netAssetValue"": 1000000
                        },
                        {
                            ""id"": 3,
                            ""fundId"": 1,
                            ""navPrice"": 10.75,
                            ""marketPrice"": 10.50,
                            ""navDate"": ""2025-01-02T00:00:00"",
                            ""dailyChangePercent"": 0.6,
                            ""netAssetValue"": 1000000
                        }
                    ]
                }
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
            mediaItem.GetValue<string>("umbracoFile").Returns(testFile);

            _fixture.MediaService.GetRootMedia().Returns(new[] { mediaItem });

            // Act
            var service = _fixture.CreateService();
            var result = await service.GetNavHistoryAsync("TESTFUND");

            // Assert - Verify list is ordered by date descending
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(11.00m, result[0].NavPrice); // Most recent (2025-01-03)
            Assert.Equal(10.75m, result[1].NavPrice); // Middle (2025-01-02)
            Assert.Equal(10.50m, result[2].NavPrice); // Oldest (2025-01-01)
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
        var testJson = @"{
            ""funds"": [
                {
                    ""id"": 1,
                    ""fundName"": ""Test Fund"",
                    ""tickerCode"": ""TEST"",
                    ""navPrice"": 10.50,
                    ""marketPrice"": 10.25,
                    ""holdInTrust"": ""Yes"",
                    ""historicalNav"": []
                }
            ]
        }";

        var testDir = Path.Combine(Path.GetTempPath(), "navhistory-tests-null");
        Directory.CreateDirectory(testDir);
        var testFile = Path.Combine(testDir, "funds.json");

        try
        {
            System.IO.File.WriteAllText(testFile, testJson);

            var mediaItem = Substitute.For<IMedia>();
            mediaItem.Name.Returns("funds.json");
            mediaItem.GetValue<string>("umbracoFile").Returns(testFile);

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
    public MediaFileManager MediaFileManager { get; }
    public IServiceProvider ServiceProvider { get; }

    public NavHistoryTestFixture()
    {
        // Set up service collection with DI
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging();

        // Register the services (using NSubstitute for interfaces we can't instantiate)
        MediaService = Substitute.For<IMediaService>();
        MediaFileManager = Substitute.For<MediaFileManager>();

        services.AddSingleton(MediaService);
        services.AddSingleton(MediaFileManager);
        services.AddScoped<IFundsJsonService, FundsJsonService>();
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
