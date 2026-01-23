using Microsoft.EntityFrameworkCore;
using Umbraco13.Data;
using Umbraco13.Models;

namespace Umbraco13.Services;

public class FundService : IFundService
{
    private readonly AppDbContext _context;

    public FundService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Fund>> GetAllFundsAsync()
    {
        return await _context.Funds.ToListAsync();
    }
}
