using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Umbraco13.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if API key authentication is enabled
        var enabled = _configuration.GetValue<bool>("ApiKeyAuthentication:Enabled", true);
        if (!enabled)
        {
            await _next(context);
            return;
        }

        // Check if the endpoint allows anonymous access
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            // Check for AllowAnonymous attribute
            var allowAnonymous = endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>();
            if (allowAnonymous != null)
            {
                await _next(context);
                return;
            }
        }

        // Also check if the path is excluded (e.g., umbraco backoffice)
        var excludedPaths = _configuration.GetSection("ApiKeyAuthentication:ExcludedPaths").Get<string[]>()
            ?? new[] { "/umbraco", "/api", "/media", "/App_Plugins" };

        var requestPath = context.Request.Path.Value ?? "";
        // Treat empty path as root path
        if (string.IsNullOrEmpty(requestPath))
        {
            requestPath = "/";
        }

        _logger.LogInformation("Checking path: {RequestPath} against excluded paths: {ExcludedPaths}",
            requestPath, string.Join(", ", excludedPaths));

        if (excludedPaths.Any(path => requestPath.StartsWith(path, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogInformation("Path {RequestPath} is excluded, bypassing API key check", requestPath);
            await _next(context);
            return;
        }

        // Check for API key in header or query string
        var apiKey = GetApiKeyFromRequest(context);

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("API key missing for request to {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "API key is required" });
            return;
        }

        // Validate the API key
        var validApiKey = _configuration.GetValue<string>("ApiKeyAuthentication:ApiKey");
        if (string.IsNullOrEmpty(validApiKey))
        {
            _logger.LogWarning("API key not configured in appsettings");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "API key authentication not configured" });
            return;
        }

        if (apiKey != validApiKey)
        {
            _logger.LogWarning("Invalid API key provided for request to {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid API key" });
            return;
        }

        // API key is valid, create a claims principal and set the user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "ApiKeyUser"),
            new Claim("ApiKey", apiKey),
            new Claim(ClaimTypes.Role, "ApiUser")
        };

        var identity = new ClaimsIdentity(claims, "ApiKey");
        var principal = new ClaimsPrincipal(identity);
        context.User = principal;

        await _next(context);
    }

    private static string? GetApiKeyFromRequest(HttpContext context)
    {
        // Try to get from header first
        if (context.Request.Headers.TryGetValue("X-API-Key", out var headerKey))
        {
            return headerKey.ToString();
        }

        // Try Authorization header with Bearer token
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var authValue = authHeader.ToString();
            if (authValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authValue["Bearer ".Length..].Trim();
            }
            if (authValue.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase))
            {
                return authValue["ApiKey ".Length..].Trim();
            }
        }

        // Try query string
        if (context.Request.Query.TryGetValue("api_key", out var queryKey))
        {
            return queryKey.ToString();
        }

        return null;
    }
}
