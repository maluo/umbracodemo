using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco13.Authorization;
using Umbraco13.Services;

namespace Umbraco13.Controllers;

[Route("funds")]
public class FundsHtmlPdfController : ControllerBase
{
    private readonly IFundService _fundService;
    private readonly ILogger<FundsHtmlPdfController> _logger;
    private readonly IHtmlPdfPrintService _htmlPdfPrintService;

    public FundsHtmlPdfController(IFundService fundService, ILogger<FundsHtmlPdfController> logger, IHtmlPdfPrintService htmlPdfPrintService)
    {
        _fundService = fundService;
        _logger = logger;
        _htmlPdfPrintService = htmlPdfPrintService;
    }

    [HttpGet("exporttohtmlpdf")]
    [ValidateDownloadToken("html-pdf")]
    public async Task<IActionResult> ExportToHtmlPdf()
    {
        try
        {
            var funds = await _fundService.GetAllFundsAsync();

            var columns = new List<HtmlPdfColumnDefinition>
            {
                new() { PropertyName = "FundName", HeaderText = "Fund Name", Alignment = "left" },
                new() { PropertyName = "TickerCode", HeaderText = "Ticker", Alignment = "center" },
                new() { PropertyName = "NavPrice", HeaderText = "NAV Price", Format = "F2", Alignment = "right" },
                new() { PropertyName = "MarketPrice", HeaderText = "Market Price", Format = "F2", Alignment = "right" },
                new() { PropertyName = "HoldInTrust", HeaderText = "Hold In Trust", Alignment = "left" }
            };

            var options = new HtmlPdfExportOptions
            {
                ReportTitle = "FUND SUMMARY REPORT\nThis report provides a summary of all funds including their NAV and market prices.",
                Disclaimer = "This document is confidential and intended for internal use only.",
                FooterText = "Total Funds: " + funds.Count
            };

            var pdfBytes = _htmlPdfPrintService.ExportToPdf(funds, columns, options);

            return File(
                pdfBytes,
                "application/pdf",
                $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting funds to HTML PDF: {Message}", ex.Message);
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
