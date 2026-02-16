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

    /// <summary>
    /// Fetches funds data from the Umbraco Delivery API
    /// </summary>
    Task<List<FundJsonItem>> GetFundsFromDeliveryApiAsync(string guid);

    /// <summary>
    /// Fetches fund data by ID from the Umbraco Delivery API and parses the JSON properties.
    /// </summary>
    Task<List<FundJsonItem>> GetFundsByIdFromDeliveryApiAsync(Guid id);
}
