using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco13.Services;

namespace Umbraco13.ViewComponents;

public class FundsJsonViewComponent : ViewComponent
{
    private readonly IMediaService _mediaService;
    private readonly INavHistoryService _navHistoryService;

    public FundsJsonViewComponent(IMediaService mediaService, INavHistoryService navHistoryService)
    {
        _mediaService = mediaService;
        _navHistoryService = navHistoryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Just return the view, the JSON will be loaded via AJAX
        return View();
    }
}
