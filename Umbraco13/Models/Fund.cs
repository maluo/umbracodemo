using System.ComponentModel.DataAnnotations;

namespace Umbraco13.Models;

public class Fund
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string FundName { get; set; } = string.Empty;

    [Required]
    public string TickerCode { get; set; } = string.Empty;

    public decimal NavPrice { get; set; }

    public decimal MarketPrice { get; set; }

    public string HoldInTrust { get; set; } = string.Empty;
}
