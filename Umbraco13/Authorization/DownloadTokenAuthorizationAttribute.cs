using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Umbraco13.Services;

namespace Umbraco13.Authorization;

public class DownloadTokenAuthorizationAttribute : TypeFilterAttribute
{
    public DownloadTokenAuthorizationAttribute(string downloadType) : base(typeof(DownloadTokenFilter))
    {
        Arguments = new object[] { downloadType };
    }

    private class DownloadTokenFilter : IAuthorizationFilter
    {
        private readonly string _downloadType;
        private readonly IDownloadTokenService _tokenService;
        private readonly ILogger<DownloadTokenFilter> _logger;

        public DownloadTokenFilter(
            string downloadType,
            IDownloadTokenService tokenService,
            ILogger<DownloadTokenFilter> logger)
        {
            _downloadType = downloadType;
            _tokenService = tokenService;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if user is already authenticated (API key or JWT)
            if (context.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                return; // Already authenticated, allow access
            }

            // Check for download token in query string
            if (context.HttpContext.Request.Query.TryGetValue("token", out var tokenValue))
            {
                var token = tokenValue.ToString();
                if (_tokenService.ValidateDownloadToken(token, _downloadType))
                {
                    return; // Valid token, allow access
                }

                _logger.LogWarning("Invalid download token for {DownloadType}", _downloadType);
            }

            // Neither authenticated nor valid token
            context.Result = new UnauthorizedObjectResult(new { error = "Valid API key or download token required" });
        }
    }
}
