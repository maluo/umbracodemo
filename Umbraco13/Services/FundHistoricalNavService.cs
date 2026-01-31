using Microsoft.EntityFrameworkCore;
using Umbraco13.Data;
using Umbraco13.Models;

namespace Umbraco13.Services;

/// <summary>
/// Service for managing fund historical NAV data
/// </summary>
public class FundHistoricalNavService : IFundHistoricalNavService
{
    private readonly AppDbContext _context;
    private readonly ILogger<FundHistoricalNavService> _logger;

    public FundHistoricalNavService(AppDbContext context, ILogger<FundHistoricalNavService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<FundHistoricalNav>> GetHistoricalNavsByFundIdAsync(int fundId)
    {
        try
        {
            return await _context.FundHistoricalNavs
                .Where(h => h.FundId == fundId)
                .OrderBy(h => h.NavDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting historical NAVs for fund {FundId}", fundId);
            return new List<FundHistoricalNav>();
        }
    }

    public async Task<FundHistoricalNavViewModel> GetHistoricalNavViewModelAsync(int fundId)
    {
        try
        {
            var fund = await _context.Funds.FindAsync(fundId);
            if (fund == null)
            {
                return new FundHistoricalNavViewModel
                {
                    FundId = fundId,
                    FundName = "Unknown Fund",
                    TickerCode = "N/A"
                };
            }

            var historicalNavs = await GetHistoricalNavsByFundIdAsync(fundId);

            return new FundHistoricalNavViewModel
            {
                FundId = fundId,
                FundName = fund.FundName,
                TickerCode = fund.TickerCode,
                HistoricalNavs = historicalNavs.Select(h => new FundHistoricalNavItem
                {
                    Id = h.Id,
                    NavDate = h.NavDate,
                    NavPrice = h.NavPrice,
                    MarketPrice = h.MarketPrice,
                    DailyChangePercent = h.DailyChangePercent,
                    NetAssetValue = h.NetAssetValue
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting historical NAV view model for fund {FundId}", fundId);
            return new FundHistoricalNavViewModel
            {
                FundId = fundId,
                FundName = "Error loading data",
                TickerCode = "N/A"
            };
        }
    }

    public async Task<List<FundHistoricalNav>> GetAllHistoricalNavsAsync()
    {
        try
        {
            return await _context.FundHistoricalNavs
                .OrderBy(h => h.FundId)
                .ThenBy(h => h.NavDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all historical NAVs");
            return new List<FundHistoricalNav>();
        }
    }
}
