using Microsoft.EntityFrameworkCore;
using FundsApi.Data;
using FundsApi.Models;

namespace FundsApi.Services;

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
