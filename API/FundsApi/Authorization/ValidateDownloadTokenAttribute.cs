using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FundsApi.Services;

namespace FundsApi.Authorization;

public class ValidateDownloadTokenAttribute : ActionFilterAttribute
{
    private readonly string _downloadType;

    public ValidateDownloadTokenAttribute(string downloadType)
    {
        _downloadType = downloadType;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var tokenService = context.HttpContext.RequestServices.GetRequiredService<IDownloadTokenService>();

        // Check for download token in query string
        if (context.HttpContext.Request.Query.TryGetValue("token", out var tokenValue))
        {
            if (tokenService.ValidateDownloadToken(tokenValue.ToString(), _downloadType))
            {
                return; // Valid token
            }
        }

        // No valid token - return JSON error
        context.Result = new JsonResult(new { error = "Valid download token required" })
        {
            StatusCode = 401
        };
    }
}
