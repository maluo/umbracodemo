using System.Text.Json;
using Umbraco13.Models;

namespace Umbraco13.Services;

/// <summary>
/// Service that loads funds.json data once at startup and caches it in memory.
/// This eliminates the need for file I/O on each request.
/// </summary>
public class FundsJsonService : IFundsJsonService
{
    private readonly FundJsonRoot _fundsData;
    private readonly ILogger<FundsJsonService> _logger;

    public FundsJsonService(ILogger<FundsJsonService> logger)
    {
        _logger = logger;

        // Load funds.json from AppData folder during construction
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Umbraco13", "AppData", "funds.json");

        if (!File.Exists(jsonPath))
        {
            _logger.LogError("funds.json not found at {FilePath}", jsonPath);
            _fundsData = new FundJsonRoot();
            return;
        }

        try
        {
            var jsonContent = File.ReadAllText(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _fundsData = JsonSerializer.Deserialize<FundJsonRoot>(jsonContent, options) ?? new FundJsonRoot();
            _logger.LogInformation("Loaded {FundCount} funds from funds.json", _fundsData.Funds.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading funds.json from {FilePath}", jsonPath);
            _fundsData = new FundJsonRoot();
        }
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
