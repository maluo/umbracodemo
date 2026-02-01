using FundsApi.Models;

namespace FundsApi.Services;

public interface IFundService
{
    Task<List<Fund>> GetAllFundsAsync();
}
