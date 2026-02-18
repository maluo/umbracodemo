using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco13.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core;

namespace Umbraco13.Services;

/// <summary>
/// Service that loads funds.json data from Umbraco media library once at startup and caches it in memory.
/// This eliminates the need for file I/O on each request.
/// </summary>
public class FundsJsonService : IFundsJsonService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly FundJsonRoot _fundsData;
    private readonly ILogger<FundsJsonService> _logger;
    private readonly MediaFileManager _mediaFileManager;
    private readonly IMediaService _mediaService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public FundsJsonService(
        ILogger<FundsJsonService> logger,
        MediaFileManager mediaFileManager,
        IMediaService mediaService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _mediaFileManager = mediaFileManager;
        _mediaService = mediaService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;

        // Load and cache funds data at startup
        _fundsData = LoadFundsData();
    }

    /// <summary>
    /// Loads funds data from Umbraco media library using MediaFileManager and IMediaService
    /// </summary>
    private FundJsonRoot LoadFundsData()
    {
        // Find the funds.json media item by searching root media items
        var rootMediaItems = _mediaService.GetRootMedia();
        if (rootMediaItems == null || !rootMediaItems.Any())
        {
            _logger.LogError("No root media items found");
            return new FundJsonRoot();
        }

        var fundsJsonMedia = FindFundsJsonMedia(rootMediaItems);
        if (fundsJsonMedia == null)
        {
            _logger.LogError("funds.json not found in Umbraco media library");
            return new FundJsonRoot();
        }

        var fileName = fundsJsonMedia.GetValue<string>(Constants.Conventions.Media.File);
        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogError("Media item {MediaId} does not have a valid file property", fundsJsonMedia.Id);
            return new FundJsonRoot();
        }

        // Check if the file exists in the abstract file system
        if (!_mediaFileManager.FileSystem.FileExists(fileName))
        {
            _logger.LogError("File {FileName} does not exist in media file system", fileName);
            return new FundJsonRoot();
        }

        // Read the file content
        using var stream = _mediaFileManager.FileSystem.OpenFile(fileName);
        using var reader = new StreamReader(stream);

        try
        {
            var jsonContent = reader.ReadToEnd();
            var data = JsonSerializer.Deserialize<FundJsonRoot>(jsonContent, _jsonOptions);
            if (data == null)
            {
                _logger.LogError("Failed to deserialize funds.json from media file");
                return new FundJsonRoot();
            }

            _logger.LogInformation("Successfully loaded {FundCount} funds from funds.json", data.Funds.Count);
            return data;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON format in funds.json");
            return new FundJsonRoot();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading funds.json from media file system");
            return new FundJsonRoot();
        }
    }

    /// <summary>
    /// Recursively searches for funds.json in the media library tree
    /// </summary>
    private IMedia? FindFundsJsonMedia(IEnumerable<IMedia> mediaItems)
    {
        foreach (var media in mediaItems)
        {
            // Check if this is the funds.json file
            if (IsFundsJsonFile(media))
            {
                _logger.LogInformation("Found funds.json media item: {MediaName} (ID: {MediaId})", media.Name, media.Id);
                return media;
            }

            // Recursively search children
            var children = _mediaService.GetPagedChildren(media.Id, 0, int.MaxValue, out _).ToList();
            if (children.Any())
            {
                var found = FindFundsJsonMedia(children);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Checks if a media item is the funds.json file
    /// </summary>
    private bool IsFundsJsonFile(IMedia media)
    {
        if (!string.IsNullOrEmpty(media.Name) && media.Name.Equals("funds.json", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var fileName = media.GetValue<string>(Constants.Conventions.Media.File);
        return fileName?.EndsWith("funds.json", StringComparison.OrdinalIgnoreCase) ?? false;
    }

    /// <inheritdoc />
    public FundJsonRoot GetFundsData()
    {
        return _fundsData;
    }

    /// <inheritdoc />
    public List<NavHistoryEntry> GetNavHistory(string tickerCode)
    {
        if (string.IsNullOrWhiteSpace(tickerCode))
        {
            return new List<NavHistoryEntry>();
        }

        var fund = _fundsData.Funds
            .FirstOrDefault(f => f.TickerCode.Equals(tickerCode, StringComparison.OrdinalIgnoreCase));

        if (fund == null)
        {
            _logger.LogWarning("Fund with ticker code {TickerCode} not found", tickerCode);
            return new List<NavHistoryEntry>();
        }

        var history = fund.HistoricalNav
            .Select(nav => new NavHistoryEntry
            {
                TickerCode = tickerCode,
                Date = nav.NavDate,
                NavPrice = nav.NavPrice,
                MarketPrice = nav.MarketPrice
            })
            .OrderByDescending(e => e.Date)
            .ToList();

        _logger.LogInformation("Retrieved {Count} NAV history entries for {TickerCode}", history.Count, tickerCode);
        return history;
    }

    /// <inheritdoc />
    public async Task<List<FundJsonItem>> GetFundsFromDeliveryApiAsync(string guid)
    {
        // Validate GUID parameter
        if (string.IsNullOrWhiteSpace(guid))
        {
            _logger.LogWarning("No GUID provided to GetFundsFromDeliveryApiAsync");
            return new List<FundJsonItem>();
        }

        var baseUrl = _configuration.GetValue<string>("Umbraco:CMS:DeliveryApi:DeliveryApiUrl")
                      ?? "http://localhost:7269/umbraco/delivery/api/v1/";
        var apiUrl = $"{baseUrl}/media/item/{guid}";
        var apiKey = _configuration.GetValue<string>("Umbraco:CMS:DeliveryApi:ApiKey") ?? "1234567890";

        try
        {
            var client = _httpClientFactory.CreateClient();
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Add("Api-Key", apiKey);
            }

            _logger.LogInformation("Fetching fund data from Delivery API: {ApiUrl}", apiUrl);
            var response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var contentItem = JsonSerializer.Deserialize<DeliveryApiContentItem>(jsonContent, _jsonOptions);

            if (contentItem?.Properties == null)
            {
                _logger.LogWarning("No properties found in Delivery API response for item {Guid}", guid);
                return new List<FundJsonItem>();
            }

            // The item may contain funds list in its properties (e.g., from an uploaded funds.json)
            // We need to find which property holds the data.
            foreach (var prop in contentItem.Properties)
            {
                var propValue = prop.Value?.ToString();
                if (string.IsNullOrEmpty(propValue)) continue;

                // Check if this property looks like JSON containing funds
                if (propValue.TrimStart().StartsWith("{") || propValue.TrimStart().StartsWith("["))
                {
                    try
                    {
                        var root = JsonSerializer.Deserialize<FundJsonRoot>(propValue, _jsonOptions);
                        if (root?.Funds != null && root.Funds.Any())
                        {
                            _logger.LogInformation("Successfully parsed {Count} funds from property '{PropName}'", root.Funds.Count, prop.Key);
                            return root.Funds;
                        }

                        // Try deserializing as a direct list
                        var list = JsonSerializer.Deserialize<List<FundJsonItem>>(propValue, _jsonOptions);
                        if (list != null && list.Any())
                        {
                            _logger.LogInformation("Successfully parsed {Count} funds from property '{PropName}' as list", list.Count, prop.Key);
                            return list;
                        }
                    }
                    catch (JsonException)
                    {
                        // Not the property we are looking for or malformed JSON
                        continue;
                    }
                }
            }

            _logger.LogWarning("No funds data found in any property of item {Guid}", guid);
            return new List<FundJsonItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching funds from Delivery API at {ApiUrl}", apiUrl);
            return new List<FundJsonItem>();
        }
    }

    /// <inheritdoc />
    public async Task<List<FundJsonItem>> GetFundsByIdFromDeliveryApiAsync(Guid id)
    {
        var baseUrl = _configuration.GetValue<string>("Umbraco:CMS:DeliveryApi:DeliveryApiUrl")
                      ?? "http://localhost:7269/umbraco/delivery/api/v1";

        var apiUrl = $"{baseUrl}/content/item/{id}";
        var apiKey = _configuration.GetValue<string>("Umbraco:CMS:DeliveryApi:ApiKey") ?? "1234567890";

        try
        {
            var client = _httpClientFactory.CreateClient();
            if (!string.IsNullOrEmpty(apiKey))
            {
                // Umbraco Delivery API uses Api-Key header
                client.DefaultRequestHeaders.Add("Api-Key", apiKey);
            }

            _logger.LogInformation("Fetching fund data from Delivery API: {ApiUrl}", apiUrl);
            var response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var contentItem = JsonSerializer.Deserialize<DeliveryApiContentItem>(jsonContent, _jsonOptions);

            if (contentItem?.Properties == null)
            {
                _logger.LogWarning("No properties found in Delivery API response for item {Id}", id);
                return new List<FundJsonItem>();
            }

            // The user requested to parse the returned json objects into a list.
            // If the item itself contains the funds list in its properties (e.g. from an uploaded funds.json)
            // we need to find which property holds the data.
            
            foreach (var prop in contentItem.Properties)
            {
                var propValue = prop.Value?.ToString();
                if (string.IsNullOrEmpty(propValue)) continue;

                // Check if this property looks like JSON containing funds
                if (propValue.TrimStart().StartsWith("{") || propValue.TrimStart().StartsWith("["))
                {
                    try
                    {
                        var root = JsonSerializer.Deserialize<FundJsonRoot>(propValue, _jsonOptions);
                        if (root?.Funds != null && root.Funds.Any())
                        {
                            _logger.LogInformation("Successfully parsed {Count} funds from property '{PropName}'", root.Funds.Count, prop.Key);
                            return root.Funds;
                        }

                        // Try deserializing as a direct list
                        var list = JsonSerializer.Deserialize<List<FundJsonItem>>(propValue, _jsonOptions);
                        if (list != null && list.Any())
                        {
                            _logger.LogInformation("Successfully parsed {Count} funds from property '{PropName}' as list", list.Count, prop.Key);
                            return list;
                        }
                    }
                    catch (JsonException)
                    {
                        // Not the property we are looking for or malformed JSON
                        continue;
                    }
                }
            }

            _logger.LogWarning("Could not find fund data in any property of item {Id}", id);
            return new List<FundJsonItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching fund data from Delivery API at {ApiUrl}", apiUrl);
            return new List<FundJsonItem>();
        }
    }

    private string? GetPropertyValue(DeliveryApiContentItem item, string alias)
    {
        if (item.Properties.TryGetValue(alias, out var value))
        {
            return value?.ToString();
        }
        return null;
    }

    private decimal ParseDecimal(string? value)
    {
        if (decimal.TryParse(value, out var result))
        {
            return result;
        }
        return 0;
    }
}
