using Umbraco13.Models;

namespace Umbraco13.Services;

public interface IFundService
{
    Task<List<Fund>> GetAllFundsAsync();
}
