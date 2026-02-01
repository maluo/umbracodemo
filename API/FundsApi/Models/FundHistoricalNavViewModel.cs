namespace FundsApi.Models;

/// <summary>
/// View model for historical NAV table
/// </summary>
public class FundHistoricalNavViewModel
{
    /// <summary>
    /// Fund ID
    /// </summary>
    public int FundId { get; set; }

    /// <summary>
    /// Fund Name
    /// </summary>
    public string FundName { get; set; } = string.Empty;

    /// <summary>
    /// Ticker Code
    /// </summary>
    public string TickerCode { get; set; } = string.Empty;

    /// <summary>
    /// Historical NAV data
    /// </summary>
    public List<FundHistoricalNavItem> HistoricalNavs { get; set; } = new();

    /// <summary>
    /// Total items count
    /// </summary>
    public int TotalItems => HistoricalNavs.Count;
}

/// <summary>
/// Historical NAV item for display
/// </summary>
public class FundHistoricalNavItem
{
    public int Id { get; set; }
    public DateTime NavDate { get; set; }
    public decimal NavPrice { get; set; }
    public decimal MarketPrice { get; set; }
    public decimal DailyChangePercent { get; set; }
    public decimal NetAssetValue { get; set; }
}
