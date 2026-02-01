using FundsApi.Models;

namespace FundsApi.Services;

/// <summary>
/// Service for managing fund historical NAV data
/// </summary>
public interface IFundHistoricalNavService
{
    /// <summary>
    /// Get all historical NAV data for a specific fund
    /// </summary>
    Task<List<FundHistoricalNav>> GetHistoricalNavsByFundIdAsync(int fundId);

    /// <summary>
    /// Get historical NAV data for a specific fund with view model
    /// </summary>
    Task<FundHistoricalNavViewModel> GetHistoricalNavViewModelAsync(int fundId);

    /// <summary>
    /// Get all historical NAV data
    /// </summary>
    Task<List<FundHistoricalNav>> GetAllHistoricalNavsAsync();
}
