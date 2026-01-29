using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Umbraco13.Services;

public class DownloadTokenService : IDownloadTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DownloadTokenService> _logger;
    private readonly string _tokensFilePath;
    private readonly Timer _refreshTimer;
    private readonly int _expiryMinutes = 30;
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private string _currentVersion = string.Empty;

    public string CurrentVersion => _currentVersion;

    public DownloadTokenService(IConfiguration configuration, ILogger<DownloadTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _expiryMinutes = _configuration.GetValue<int>("DownloadTokens:ExpiryInMinutes", 30);

        // Create AppData folder if it doesn't exist
        var appDataPath = Path.Combine(Directory.GetCurrentDirectory(), "AppData");
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        _tokensFilePath = Path.Combine(appDataPath, "download-tokens.json");

        // Initialize tokens file if it doesn't exist
        InitializeTokensFile();

        // Start timer to refresh tokens every 20 minutes (starts after 20 minutes, not immediately)
        _refreshTimer = new Timer(_ => RefreshTokens(), null, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(20));
    }

    private void InitializeTokensFile()
    {
        try
        {
            _logger.LogInformation("Checking token file at {FilePath}", _tokensFilePath);

            if (!File.Exists(_tokensFilePath))
            {
                _logger.LogInformation("Token file does not exist, generating new tokens...");
                var tokenData = GenerateAllTokens();
                SaveTokensToFileSync(tokenData);
                _logger.LogInformation("Token file created successfully at {FilePath}", _tokensFilePath);
            }
            else
            {
                _logger.LogInformation("Token file already exists at {FilePath}", _tokensFilePath);
                var tokenData = LoadTokensFromFileSync();
                _currentVersion = tokenData.Version;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing tokens file: {Message}", ex.Message);
        }
    }

    private static string GenerateRandomToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        var base64 = Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        return base64.Length > 44 ? base64[..44] : base64.PadRight(44, '0');
    }

    private static string GenerateVersion() => DateTime.UtcNow.Ticks.ToString();

    private TokenFileData GenerateAllTokens()
    {
        var expiryTime = DateTime.UtcNow.AddMinutes(_expiryMinutes);
        var version = GenerateVersion();

        return new TokenFileData
        {
            Version = version,
            Tokens = new Dictionary<string, TokenData>
            {
                { "pdf", new TokenData { Token = GenerateRandomToken(), Expiry = expiryTime, Type = "pdf" } },
                { "csv", new TokenData { Token = GenerateRandomToken(), Expiry = expiryTime, Type = "csv" } },
                { "excel-free", new TokenData { Token = GenerateRandomToken(), Expiry = expiryTime, Type = "excel-free" } },
                { "excel", new TokenData { Token = GenerateRandomToken(), Expiry = expiryTime, Type = "excel" } }
            }
        };
    }

    private void SaveTokensToFileSync(TokenFileData tokenData)
    {
        try
        {
            var json = JsonSerializer.Serialize(tokenData, _jsonOptions);
            File.WriteAllText(_tokensFilePath, json);
            _currentVersion = tokenData.Version;
            _logger.LogInformation("Tokens saved to file successfully. Version: {Version}", tokenData.Version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving tokens to file: {Message}", ex.Message);
            throw;
        }
    }

    private TokenFileData LoadTokensFromFileSync()
    {
        try
        {
            if (!File.Exists(_tokensFilePath))
            {
                _logger.LogWarning("Token file does not exist at {FilePath}", _tokensFilePath);
                return new TokenFileData { Version = GenerateVersion(), Tokens = new Dictionary<string, TokenData>() };
            }

            var json = File.ReadAllText(_tokensFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("Token file is empty at {FilePath}", _tokensFilePath);
                return new TokenFileData { Version = GenerateVersion(), Tokens = new Dictionary<string, TokenData>() };
            }

            var tokenData = JsonSerializer.Deserialize<TokenFileData>(json);
            return tokenData ?? new TokenFileData { Version = GenerateVersion(), Tokens = new Dictionary<string, TokenData>() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tokens from file: {Message}", ex.Message);
            return new TokenFileData { Version = GenerateVersion(), Tokens = new Dictionary<string, TokenData>() };
        }
    }

    private void RefreshTokens()
    {
        try
        {
            _logger.LogInformation("Refreshing download tokens (overriding file)...");
            var tokenData = GenerateAllTokens();
            SaveTokensToFileSync(tokenData);
            _logger.LogInformation("Download tokens refreshed successfully. New version: {Version}, Expiry: {Expiry}",
                tokenData.Version, tokenData.Tokens["pdf"].Expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing tokens: {Message}", ex.Message);
        }
    }

    public string GenerateDownloadToken(string downloadType)
    {
        try
        {
            var tokenData = LoadTokensFromFileSync();
            _currentVersion = tokenData.Version;

            if (!tokenData.Tokens.ContainsKey(downloadType))
            {
                _logger.LogWarning("Token type {Type} not found, regenerating all tokens...", downloadType);
                RefreshTokens();
                tokenData = LoadTokensFromFileSync();
                _currentVersion = tokenData.Version;
            }

            if (tokenData.Tokens.TryGetValue(downloadType, out var data))
            {
                // Check if token is expired
                if (DateTime.UtcNow > data.Expiry)
                {
                    _logger.LogWarning("Token expired at {Expiry}, refreshing...", data.Expiry);
                    RefreshTokens();
                    tokenData = LoadTokensFromFileSync();
                    _currentVersion = tokenData.Version;
                }

                return tokenData.Tokens[downloadType].Token;
            }

            _logger.LogWarning("Token type {Type} not found after refresh", downloadType);
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating download token: {Message}", ex.Message);
            return string.Empty;
        }
    }

    public bool ValidateDownloadToken(string token, string downloadType)
    {
        try
        {
            var tokenData = LoadTokensFromFileSync();

            if (!tokenData.Tokens.TryGetValue(downloadType, out var data))
            {
                _logger.LogWarning("Token type {Type} not found", downloadType);
                return false;
            }

            // Check if token matches
            if (data.Token != token)
            {
                _logger.LogWarning("Token mismatch for type {Type}", downloadType);
                return false;
            }

            // Check if token has expired
            if (DateTime.UtcNow > data.Expiry)
            {
                _logger.LogWarning("Token expired at {ExpiryTime}", data.Expiry);
                return false;
            }

            _logger.LogInformation("Token validated successfully for type {Type}", downloadType);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating download token: {Message}", ex.Message);
            return false;
        }
    }

    private class TokenFileData
    {
        public string Version { get; set; } = string.Empty;
        public Dictionary<string, TokenData> Tokens { get; set; } = new();
    }

    private class TokenData
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
