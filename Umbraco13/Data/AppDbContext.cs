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

        // Seed data
        modelBuilder.Entity<Fund>().HasData(
            new Fund { Id = 1, FundName = "Tech Growth Fund", TickerCode = "TGF123", NavPrice = 45.67m, MarketPrice = 46.20m, HoldInTrust = "Yes" },
            new Fund { Id = 2, FundName = "Dividend Yield Fund", TickerCode = "DYF456", NavPrice = 32.10m, MarketPrice = 31.95m, HoldInTrust = "No" },
            new Fund { Id = 3, FundName = "Bond Income Fund", TickerCode = "BIF789", NavPrice = 28.50m, MarketPrice = 28.75m, HoldInTrust = "Yes" },
            new Fund { Id = 4, FundName = "Emerging Markets Fund", TickerCode = "EMF101", NavPrice = 55.30m, MarketPrice = 55.80m, HoldInTrust = "No" }
        );
    }
}
