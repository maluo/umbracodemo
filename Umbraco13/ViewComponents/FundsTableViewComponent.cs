using Microsoft.AspNetCore.Mvc;
using Umbraco13.Helpers;
using Umbraco13.Models;
using Umbraco13.Services;

namespace Umbraco13.ViewComponents;

public class FundsTableViewComponent : ViewComponent
{
    private readonly IFundService _fundService;

    public FundsTableViewComponent(IFundService fundService)
    {
        _fundService = fundService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var funds = await _fundService.GetAllFundsAsync();
        var tableData = FundTableConverter.ToTableData(funds);

        var viewModel = new FundsTableViewModel
        {
            TableData = tableData,
            Funds = funds.ToList()
        };

        return View(viewModel);
    }
}
