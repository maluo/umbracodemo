using System.Net;

namespace Umbraco13.Middleware;

/// <summary>
/// Middleware to restrict backoffice access to whitelisted IP addresses
/// </summary>
public class IpWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpWhitelistMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public IpWhitelistMiddleware(RequestDelegate next, ILogger<IpWhitelistMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var enabled = _configuration.GetValue<bool>("IpWhitelist:Enabled", false);

        if (!enabled)
        {
            await _next(context);
            return;
        }

        var allowedIps = _configuration.GetSection("IpWhitelist:AllowedIps").Get<string[]>();
        var path = context.Request.Path.Value ?? "";

        // Only apply to backoffice
        if (path.StartsWith("/umbraco", StringComparison.OrdinalIgnoreCase))
        {
            var remoteIp = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(remoteIp))
            {
                _logger.LogWarning("Unable to determine remote IP for backoffice access");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            // Allow localhost for development
            if (IsLocalhost(remoteIp))
            {
                await _next(context);
                return;
            }

            // Check if IP is in whitelist
            if (allowedIps == null || allowedIps.Length == 0 || !IsIpAllowed(remoteIp, allowedIps))
            {
                _logger.LogWarning("Access denied to backoffice from IP: {RemoteIp}", remoteIp);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }

        await _next(context);
    }

    private static bool IsLocalhost(string ip) =>
        ip == "127.0.0.1" || ip == "::1" || ip == "localhost";

    private static bool IsIpAllowed(string remoteIp, string[] allowedIps)
    {
        // Direct match
        if (allowedIps.Contains(remoteIp, StringComparer.OrdinalIgnoreCase))
            return true;

        // CIDR notation support for IP ranges
        foreach (var allowedIp in allowedIps)
        {
            if (allowedIp.Contains('/'))
            {
                if (IsIpInCidrRange(remoteIp, allowedIp))
                    return true;
            }
        }

        return false;
    }

    private static bool IsIpInCidrRange(string ipToCheck, string cidr)
    {
        var parts = cidr.Split('/');
        if (parts.Length != 2)
            return false;

        if (!IPAddress.TryParse(ipToCheck, out var ipAddress))
            return false;

        if (!IPAddress.TryParse(parts[0], out var cidrIp))
            return false;

        if (!int.TryParse(parts[1], out var prefixLength))
            return false;

        var ipBytes = ipAddress.GetAddressBytes();
        var cidrBytes = cidrIp.GetAddressBytes();

        if (ipBytes.Length != cidrBytes.Length)
            return false;

        int relevantBytes = prefixLength / 8;
        int remainingBits = prefixLength % 8;

        for (int i = 0; i < relevantBytes; i++)
        {
            if (ipBytes[i] != cidrBytes[i])
                return false;
        }

        if (remainingBits > 0 && relevantBytes < ipBytes.Length)
        {
            byte mask = (byte)(0xFF << (8 - remainingBits));
            if ((ipBytes[relevantBytes] & mask) != (cidrBytes[relevantBytes] & mask))
                return false;
        }

        return true;
    }
}
