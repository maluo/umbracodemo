namespace Umbraco13.Models;

public class NavHistoryEntry
{
    public string TickerCode { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal NavPrice { get; set; }
    public decimal? MarketPrice { get; set; }
}

public class NavHistoryViewModel
{
    public string TickerCode { get; set; } = string.Empty;
    public List<NavHistoryEntry> History { get; set; } = new();
}
