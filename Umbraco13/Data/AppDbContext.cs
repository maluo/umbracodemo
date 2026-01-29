using Microsoft.EntityFrameworkCore;
using Umbraco13.Models;

namespace Umbraco13.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Fund> Funds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data - original 4 funds
        modelBuilder.Entity<Fund>().HasData(
            new Fund { Id = 1, FundName = "Tech Growth Fund", TickerCode = "TGF123", NavPrice = 45.67m, MarketPrice = 46.20m, HoldInTrust = "Yes" },
            new Fund { Id = 2, FundName = "Dividend Yield Fund", TickerCode = "DYF456", NavPrice = 32.10m, MarketPrice = 31.95m, HoldInTrust = "No" },
            new Fund { Id = 3, FundName = "Bond Income Fund", TickerCode = "BIF789", NavPrice = 28.50m, MarketPrice = 28.75m, HoldInTrust = "Yes" },
            new Fund { Id = 4, FundName = "Emerging Markets Fund", TickerCode = "EMF101", NavPrice = 55.30m, MarketPrice = 55.80m, HoldInTrust = "No" }
        );

        // Additional fake fund data (20 more funds)
        modelBuilder.Entity<Fund>().HasData(
            new Fund { Id = 5, FundName = "Global Equity Fund", TickerCode = "GEF202", NavPrice = 62.45m, MarketPrice = 62.80m, HoldInTrust = "Yes" },
            new Fund { Id = 6, FundName = "Small Cap Opportunities", TickerCode = "SCO301", NavPrice = 38.90m, MarketPrice = 39.15m, HoldInTrust = "No" },
            new Fund { Id = 7, FundName = "Real Estate Investment", TickerCode = "REI404", NavPrice = 125.50m, MarketPrice = 126.75m, HoldInTrust = "Yes" },
            new Fund { Id = 8, FundName = "Healthcare Innovators", TickerCode = "HCI505", NavPrice = 78.25m, MarketPrice = 77.90m, HoldInTrust = "No" },
            new Fund { Id = 9, FundName = "Sustainable Energy Fund", TickerCode = "SEF606", NavPrice = 42.80m, MarketPrice = 43.10m, HoldInTrust = "Yes" },
            new Fund { Id = 10, FundName = "Consumer Staples Plus", TickerCode = "CSP707", NavPrice = 55.60m, MarketPrice = 55.95m, HoldInTrust = "No" },
            new Fund { Id = 11, FundName = "Infrastructure Development", TickerCode = "IDD808", NavPrice = 89.30m, MarketPrice = 90.15m, HoldInTrust = "Yes" },
            new Fund { Id = 12, FundName = "Asian Growth Markets", TickerCode = "AGM909", NavPrice = 67.45m, MarketPrice = 67.80m, HoldInTrust = "No" },
            new Fund { Id = 13, FundName = "Fixed Income Securities", TickerCode = "FIS011", NavPrice = 22.75m, MarketPrice = 22.90m, HoldInTrust = "Yes" },
            new Fund { Id = 14, FundName = "Technology Leaders", TickerCode = "TLL112", NavPrice = 95.20m, MarketPrice = 96.40m, HoldInTrust = "No" },
            new Fund { Id = 15, FundName = "Balanced Portfolio Fund", TickerCode = "BPF213", NavPrice = 48.35m, MarketPrice = 48.70m, HoldInTrust = "Yes" },
            new Fund { Id = 16, FundName = "International Value Fund", TickerCode = "IVF314", NavPrice = 56.80m, MarketPrice = 57.05m, HoldInTrust = "No" },
            new Fund { Id = 17, FundName = "Gold & Precious Metals", TickerCode = "GPM415", NavPrice = 110.25m, MarketPrice = 111.50m, HoldInTrust = "Yes" },
            new Fund { Id = 18, FundName = "Agribusiness Fund", TickerCode = "ABF516", NavPrice = 41.90m, MarketPrice = 42.25m, HoldInTrust = "No" },
            new Fund { Id = 19, FundName = "Communications Fund", TickerCode = "CMF617", NavPrice = 35.60m, MarketPrice = 35.85m, HoldInTrust = "Yes" },
            new Fund { Id = 20, FundName = "Utilities Income Fund", TickerCode = "UIF718", NavPrice = 29.45m, MarketPrice = 29.70m, HoldInTrust = "No" },
            new Fund { Id = 21, FundName = "Consumer Disc Fund", TickerCode = "CDF819", NavPrice = 52.30m, MarketPrice = 52.65m, HoldInTrust = "Yes" },
            new Fund { Id = 22, FundName = "Industrial Growth Fund", TickerCode = "IGF920", NavPrice = 44.85m, MarketPrice = 45.10m, HoldInTrust = "No" },
            new Fund { Id = 23, FundName = "Financial Sector Fund", TickerCode = "FSF121", NavPrice = 38.55m, MarketPrice = 38.80m, HoldInTrust = "Yes" },
            new Fund { Id = 24, FundName = "Multi-Asset Income", TickerCode = "MAI222", NavPrice = 33.70m, MarketPrice = 33.95m, HoldInTrust = "No" }
        );
    }
}
