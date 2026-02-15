using Umbraco13.Models;
using Umbraco13.Services;

namespace Umbraco13.Services;

public class NavHistoryService : INavHistoryService
{
    private readonly IFundsJsonService _fundsJsonService;
    private readonly ILogger<NavHistoryService> _logger;

    public NavHistoryService(
        IFundsJsonService fundsJsonService,
        ILogger<NavHistoryService> logger)
    {
        _fundsJsonService = fundsJsonService;
        _logger = logger;
    }

    public Task<List<NavHistoryEntry>?> GetNavHistoryAsync(string tickerCode)
    {
        if (string.IsNullOrWhiteSpace(tickerCode))
        {
            return Task.FromResult<List<NavHistoryEntry>?>(null);
        }

        try
        {
            var history = _fundsJsonService.GetNavHistory(tickerCode);
            return Task.FromResult<List<NavHistoryEntry>?>(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading NAV history for {TickerCode}", tickerCode);
            return Task.FromResult<List<NavHistoryEntry>?>(null);
        }
    }
}
