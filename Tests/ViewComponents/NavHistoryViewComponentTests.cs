using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Umbraco13.Models;
using Umbraco13.Services;
using Umbraco13.ViewComponents;

namespace Umbraco13.Tests.ViewComponents;

public class NavHistoryViewComponentTests
{
    private readonly INavHistoryService _navHistoryService;
    private readonly NavHistoryViewComponent _sut;

    public NavHistoryViewComponentTests()
    {
        _navHistoryService = Substitute.For<INavHistoryService>();
        _sut = new NavHistoryViewComponent(_navHistoryService);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsViewWithListOfEntries()
    {
        // Arrange
        var ticker = "TESTFUND";
        var history = new List<NavHistoryEntry>
        {
            new() { TickerCode = ticker, Date = new DateTime(2025, 1, 1), NavPrice = 10.50m }
        };

        _navHistoryService.GetNavHistoryAsync(ticker).Returns(history);

        // Act
        var result = await _sut.InvokeAsync(ticker);

        // Assert
        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var model = Assert.IsType<NavHistoryViewModel>(viewResult.ViewData?.Model);

        Assert.NotNull(model);
        Assert.Equal(ticker, model.TickerCode);
        Assert.Single(model.History);
    }

    [Fact]
    public async Task InvokeAsync_WhenServiceReturnsNull_ReturnsEmptyList()
    {
        // Arrange
        var ticker = "TESTFUND";
        _navHistoryService.GetNavHistoryAsync(ticker).Returns((List<NavHistoryEntry>?)null);

        // Act
        var result = await _sut.InvokeAsync(ticker);

        // Assert
        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var model = Assert.IsType<NavHistoryViewModel>(viewResult.ViewData?.Model);

        Assert.NotNull(model);
        Assert.Equal(ticker, model.TickerCode);
        Assert.Empty(model.History);
    }

    [Fact]
    public async Task InvokeAsync_PassesTickerToService()
    {
        // Arrange
        var ticker = "TESTFUND";
        _navHistoryService.GetNavHistoryAsync(ticker).Returns(new List<NavHistoryEntry>());

        // Act
        await _sut.InvokeAsync(ticker);

        // Assert
        await _navHistoryService.Received(1).GetNavHistoryAsync(ticker);
    }
}
