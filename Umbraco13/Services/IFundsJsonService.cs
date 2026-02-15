using Umbraco13.Models;

namespace Umbraco13.Services;

public interface IFundsJsonService
{
    /// <summary>
    /// Gets the funds data from funds.json (loaded once at startup)
    /// </summary>
    FundJsonRoot GetFundsData();

    /// <summary>
    /// Gets historical NAV data for a specific ticker code
    /// </summary>
    List<NavHistoryEntry> GetNavHistory(string tickerCode);
}
