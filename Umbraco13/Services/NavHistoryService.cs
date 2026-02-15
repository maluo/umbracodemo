using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco13.Models;

namespace Umbraco13.Services;

public class NavHistoryService : INavHistoryService
{
    private readonly IMediaService _mediaService;
    private readonly ILogger<NavHistoryService> _logger;

    public NavHistoryService(
        IMediaService mediaService,
        ILogger<NavHistoryService> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<List<NavHistoryEntry>?> GetNavHistoryAsync(string tickerCode)
    {
        if (string.IsNullOrWhiteSpace(tickerCode))
        {
            return null;
        }

        try
        {
            // Find the nav history JSON file in media library
            var mediaItem = FindMediaByNameRecursive("funds.json");
            if (mediaItem == null)
            {
                _logger.LogWarning("funds.json not found in media library");
                return null;
            }

            // Get file path from media item
            var filePath = mediaItem.GetValue<string>("umbracoFile");
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            // Try to get the physical file path from the media item
            var physicalPath = mediaItem.GetValue<string>("path");

            string fullPath;
            if (!string.IsNullOrEmpty(physicalPath))
            {
                // If it's an absolute path, use it directly
                // If it's a directory, append the filename
                if (Path.IsPathRooted(physicalPath))
                {
                    var fileName = mediaItem.Name ?? "funds.json";
                    fullPath = System.IO.Directory.Exists(physicalPath)
                        ? Path.Combine(physicalPath, fileName)
                        : physicalPath;
                }
                else
                {
                    // Otherwise, construct path relative to wwwroot
                    fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", physicalPath.TrimStart('/'));
                }
            }
            else
            {
                fullPath = filePath;
            }

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning("File does not exist: {FilePath}", fullPath);
                return null;
            }

            // Read and parse JSON content
            using var stream = System.IO.File.OpenRead(fullPath);
            using var reader = new StreamReader(stream);
            var jsonContent = await reader.ReadToEndAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonData = JsonSerializer.Deserialize<JsonDocument>(jsonContent, options);
            if (jsonData == null)
            {
                return null;
            }

            // Extract NAV history for the specific ticker
            var history = new List<NavHistoryEntry>();

            if (jsonData.RootElement.TryGetProperty("funds", out var fundsElement) && fundsElement.ValueKind == JsonValueKind.Array)
            {
                var targetFund = fundsElement.EnumerateArray()
                    .FirstOrDefault(f => f.TryGetProperty("tickerCode", out var t) && 
                                        t.GetString()?.Equals(tickerCode, StringComparison.OrdinalIgnoreCase) == true);

                if (targetFund.ValueKind != JsonValueKind.Undefined && targetFund.TryGetProperty("historicalNav", out var navHistory) && navHistory.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in navHistory.EnumerateArray())
                    {
                        var entry = ParseNavEntry(item, tickerCode);
                        if (entry != null)
                        {
                            history.Add(entry);
                        }
                    }
                }
            }
            else if (jsonData.RootElement.TryGetProperty(tickerCode, out var tickerElement))
            {
                // Handle legacy structure where ticker is a direct property
                if (tickerElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in tickerElement.EnumerateArray())
                    {
                        var entry = ParseNavEntry(item, tickerCode);
                        if (entry != null)
                        {
                            history.Add(entry);
                        }
                    }
                }
                else if (tickerElement.TryGetProperty("navHistory", out var navHistory) && navHistory.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in navHistory.EnumerateArray())
                    {
                        var entry = ParseNavEntry(item, tickerCode);
                        if (entry != null)
                        {
                            history.Add(entry);
                        }
                    }
                }
            }

            _logger.LogInformation(
                "Loaded {Count} NAV history entries for {TickerCode}",
                history.Count,
                tickerCode);

            return history;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading NAV history for {TickerCode}", tickerCode);
            return null;
        }
    }

    private static NavHistoryEntry? ParseNavEntry(JsonElement item, string tickerCode)
    {
        try
        {
            var entry = new NavHistoryEntry
            {
                TickerCode = tickerCode,
                NavPrice = item.TryGetProperty("navPrice", out var navProp) ? navProp.GetDecimal() : 0,
                MarketPrice = item.TryGetProperty("marketPrice", out var mktProp) ? mktProp.GetDecimal() : null
            };

            if (item.TryGetProperty("navDate", out var dateProp))
            {
                if (dateProp.ValueKind == JsonValueKind.String)
                {
                    entry.Date = DateTime.TryParse(dateProp.GetString(), out var dt) ? dt : default;
                }
            }
            else if (item.TryGetProperty("date", out var legacyDateProp))
            {
                if (legacyDateProp.ValueKind == JsonValueKind.String)
                {
                    entry.Date = DateTime.TryParse(legacyDateProp.GetString(), out var dt) ? dt : default;
                }
            }

            return entry;
        }
        catch
        {
            return null;
        }
    }

    private IMedia? FindMediaByNameRecursive(string fileName)
    {
        try
        {
            var rootMedia = _mediaService.GetRootMedia();
            foreach (var root in rootMedia)
            {
                if (root == null) continue;

                var result = SearchMediaRecursively(root, fileName);
                if (result != null)
                    return result;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    private IMedia? SearchMediaRecursively(IMedia media, string fileName)
    {
        if (media.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
        {
            return media;
        }

        try
        {
            var children = _mediaService.GetPagedChildren(media.Id, 0, 1000, out var total);
            if (total == 0 || children == null)
                return null;

            foreach (var child in children)
            {
                if (child == null) continue;

                var result = SearchMediaRecursively(child, fileName);
                if (result != null)
                    return result;
            }
        }
        catch
        {
            // Ignore errors during recursive search
        }

        return null;
    }
}
