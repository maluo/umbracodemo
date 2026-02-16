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

    public FundsJsonService(
        ILogger<FundsJsonService> logger,
        MediaFileManager mediaFileManager,
        IMediaService mediaService)
    {
        _logger = logger;
        _mediaFileManager = mediaFileManager;
        _mediaService = mediaService;

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
}
