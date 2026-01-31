using Microsoft.AspNetCore.Mvc;
using Umbraco13.Models;
using Umbraco13.Services;

namespace Umbraco13.ViewComponents;

/// <summary>
/// View component for displaying historical NAV table
/// </summary>
public class HistoricalNavTableViewComponent : ViewComponent
{
    private readonly IFundHistoricalNavService _fundHistoricalNavService;

    public HistoricalNavTableViewComponent(IFundHistoricalNavService fundHistoricalNavService)
    {
        _fundHistoricalNavService = fundHistoricalNavService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int fundId)
    {
        var model = await _fundHistoricalNavService.GetHistoricalNavViewModelAsync(fundId);
        return View(model);
    }
}
