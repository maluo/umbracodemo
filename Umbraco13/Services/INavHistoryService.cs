using Umbraco13.Models;

namespace Umbraco13.Services;

public interface INavHistoryService
{
    Task<List<NavHistoryEntry>?> GetNavHistoryAsync(string tickerCode);
}
