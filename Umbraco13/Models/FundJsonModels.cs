using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Umbraco13.Models
{
    public class FundJsonRoot
    {
        [JsonPropertyName("funds")]
        public List<FundJsonItem> Funds { get; set; } = new();
    }

    public class FundJsonItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("fundName")]
        public string FundName { get; set; } = string.Empty;

        [JsonPropertyName("tickerCode")]
        public string TickerCode { get; set; } = string.Empty;

        [JsonPropertyName("navPrice")]
        public decimal NavPrice { get; set; }

        [JsonPropertyName("marketPrice")]
        public decimal? MarketPrice { get; set; }

        [JsonPropertyName("holdInTrust")]
        public string HoldInTrust { get; set; } = string.Empty;

        [JsonPropertyName("historicalNav")]
        public List<HistoricalNavJsonItem> HistoricalNav { get; set; } = new();

        // Future fields for documents as requested by user
        [JsonPropertyName("factsheets")]
        public List<DocumentJsonItem>? Factsheets { get; set; }

        [JsonPropertyName("prospectus")]
        public List<DocumentJsonItem>? Prospectus { get; set; }

        [JsonPropertyName("reports")]
        public List<DocumentJsonItem>? Reports { get; set; }
    }

    public class HistoricalNavJsonItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("fundId")]
        public int FundId { get; set; }

        [JsonPropertyName("navPrice")]
        public decimal NavPrice { get; set; }

        [JsonPropertyName("marketPrice")]
        public decimal? MarketPrice { get; set; }

        [JsonPropertyName("navDate")]
        public DateTime NavDate { get; set; }

        [JsonPropertyName("dailyChangePercent")]
        public decimal DailyChangePercent { get; set; }

        [JsonPropertyName("netAssetValue")]
        public long NetAssetValue { get; set; }
    }

    public class DocumentJsonItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }
    }
}
