using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.IO;
using Umbraco13.Models;

namespace Umbraco13.Services;

public class NavHistoryService : INavHistoryService
{
    private readonly IMediaService _mediaService;
    private readonly IFileSystem _mediaFileSystem;
    private readonly ILogger<NavHistoryService> _logger;

    public NavHistoryService(
        IMediaService mediaService,
        IFileSystem mediaFileSystem,
        ILogger<NavHistoryService> logger)
    {
        _mediaService = mediaService;
        _mediaFileSystem = mediaFileSystem;
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

            // Get file path and read content
            var filePath = mediaItem.GetValue<string>("umbracoFile");
            if (string.IsNullOrEmpty(filePath) || !_mediaFileSystem.FileExists(filePath))
            {
                return null;
            }

            using var stream = _mediaFileSystem.OpenFile(filePath);
            if (stream == null)
            {
                return null;
            }

            // Parse JSON
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

            if (jsonData.RootElement.TryGetProperty(tickerCode, out var tickerElement))
            {
                // Handle as array
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
                // Handle as object with nav history property
                else if (tickerElement.TryGetProperty("navHistory", out var navHistory))
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

            if (item.TryGetProperty("date", out var dateProp))
            {
                if (dateProp.ValueKind == JsonValueKind.String)
                {
                    entry.Date = DateTime.TryParse(dateProp.GetString(), out var dt) ? dt : default;
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
