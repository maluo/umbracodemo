using System.ComponentModel.DataAnnotations;

namespace Umbraco13.Models;

/// <summary>
/// Historical NAV values for funds
/// </summary>
public class FundHistoricalNav
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the Fund
    /// </summary>
    [Required]
    public int FundId { get; set; }

    /// <summary>
    /// The associated Fund
    /// </summary>
    public Fund? Fund { get; set; }

    /// <summary>
    /// NAV price on this date
    /// </summary>
    [Required]
    public decimal NavPrice { get; set; }

    /// <summary>
    /// Market price on this date
    /// </summary>
    public decimal MarketPrice { get; set; }

    /// <summary>
    /// Date of the NAV value
    /// </summary>
    [Required]
    public DateTime NavDate { get; set; }

    /// <summary>
    /// Daily percentage change from previous day
    /// </summary>
    public decimal DailyChangePercent { get; set; }

    /// <summary>
    /// Net asset value (total assets)
    /// </summary>
    public decimal NetAssetValue { get; set; }
}
