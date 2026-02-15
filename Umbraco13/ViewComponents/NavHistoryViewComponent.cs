using Microsoft.AspNetCore.Mvc;
using Umbraco13.Models;
using Umbraco13.Services;

namespace Umbraco13.ViewComponents;

public class NavHistoryViewComponent : ViewComponent
{
    private readonly INavHistoryService _navHistoryService;

    public NavHistoryViewComponent(INavHistoryService navHistoryService)
    {
        _navHistoryService = navHistoryService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string tickerCode)
    {
        var history = await _navHistoryService.GetNavHistoryAsync(tickerCode) ?? new List<NavHistoryEntry>();

        var viewModel = new NavHistoryViewModel
        {
            TickerCode = tickerCode,
            History = history
        };

        return View(viewModel);
    }
}
