using Microsoft.AspNetCore.Mvc;
using Umbraco13.Services;

namespace Umbraco13.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class FundsJsonController : Controller
{
    private readonly IFundsJsonService _fundsJsonService;
    private readonly ILogger<FundsJsonController> _logger;

    public FundsJsonController(IFundsJsonService fundsJsonService, ILogger<FundsJsonController> logger)
    {
        _fundsJsonService = fundsJsonService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetFundsJson()
    {
        try
        {
            var data = _fundsJsonService.GetFundsData();
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving funds data");
            return StatusCode(500, new { error = "Error retrieving funds data", message = ex.Message });
        }
    }
}
