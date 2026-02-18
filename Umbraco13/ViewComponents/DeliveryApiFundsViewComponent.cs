using Microsoft.AspNetCore.Mvc;
using Umbraco13.Models;
using Umbraco13.Services;

namespace Umbraco13.ViewComponents;

/// <summary>
/// ViewComponent for displaying funds data from the Umbraco Delivery API
/// </summary>
public class DeliveryApiFundsViewComponent : ViewComponent
{
    private readonly IFundsJsonService _fundsJsonService;
    private readonly ILogger<DeliveryApiFundsViewComponent> _logger;

    public DeliveryApiFundsViewComponent(
        IFundsJsonService fundsJsonService,
        ILogger<DeliveryApiFundsViewComponent> logger)
    {
        _fundsJsonService = fundsJsonService;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            _logger.LogWarning("No GUID provided to DeliveryApiFundsViewComponent");
            return View(new List<FundJsonItem>());
        }

        try
        {
            var funds = await _fundsJsonService.GetFundsFromDeliveryApiAsync(guid);
            return View(funds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading funds from Delivery API");
            return View(new List<FundJsonItem>());
        }
    }
}
