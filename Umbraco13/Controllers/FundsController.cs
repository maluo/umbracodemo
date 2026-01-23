using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;

namespace Umbraco13.Controllers;

public class FundsController : UmbracoController
{
    public async Task<IActionResult> UpdateTable()
    {
        // Return the ViewComponent result
        return ViewComponent("FundsTable");
    }
}
