using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PdfSharp.Drawing;
using System.Text;
using FundsApi.Authorization;
using FundsApi.Models.ExportModels;
using FundsApi.Services;

namespace FundsApi.Controllers;

[Route("funds")]
public class FundsController : ControllerBase
{
    private readonly IFundService _fundService;
    private readonly ILogger<FundsController> _logger;
    private readonly IDownloadTokenService _downloadTokenService;
    private readonly IConfiguration _configuration;
    private readonly IPdfExportService _pdfExportService;
    private readonly IExcelExportService _excelExportService;
    private readonly IFundHistoricalNavService _fundHistoricalNavService;

    public FundsController(IFundService fundService, ILogger<FundsController> logger, IDownloadTokenService downloadTokenService, IConfiguration configuration, IPdfExportService pdfExportService, IExcelExportService excelExportService, IFundHistoricalNavService fundHistoricalNavService)
    {
        _fundService = fundService;
        _logger = logger;
        _downloadTokenService = downloadTokenService;
        _configuration = configuration;
        _pdfExportService = pdfExportService;
        _excelExportService = excelExportService;
        _fundHistoricalNavService = fundHistoricalNavService;
    }

    [HttpGet("download-tokens")]
    [AllowAnonymous]
    public IActionResult GetDownloadTokens()
    {
        return Ok(new
        {
            version = _downloadTokenService.CurrentVersion,
            pdf = _downloadTokenService.GenerateDownloadToken("pdf"),
            csv = _downloadTokenService.GenerateDownloadToken("csv"),
            excel = _downloadTokenService.GenerateDownloadToken("excel-free"),
            excelEpPlus = _downloadTokenService.GenerateDownloadToken("excel"),
            excelGeneric = _downloadTokenService.GenerateDownloadToken("excel-generic")
        });
    }

    [HttpGet("token-version")]
    [AllowAnonymous]
    public IActionResult GetTokenVersion()
    {
        return Ok(new { version = _downloadTokenService.CurrentVersion });
    }

    [HttpGet("api-key")]
    [AllowAnonymous]
    public IActionResult GetApiKey()
    {
        var apiKey = _configuration.GetValue<string>("ApiKeyAuthentication:ApiKey");
        return Ok(new { apiKey });
    }

    [HttpGet("test-token")]
    [AllowAnonymous]
    public IActionResult TestToken(string? token = null, string type = "pdf")
    {
        if (string.IsNullOrEmpty(token))
        {
            var newToken = _downloadTokenService.GenerateDownloadToken(type);
            return Ok(new { token = newToken, type, message = "Generated new token" });
        }

        var isValid = _downloadTokenService.ValidateDownloadToken(token, type);
        return Ok(new { token, type, isValid, message = isValid ? "Token is valid" : "Token is invalid" });
    }

    [HttpGet("all-tokens")]
    [AllowAnonymous]
    public IActionResult GetAllTokens()
    {
        var tokens = _downloadTokenService.GenerateDownloadToken("all");
        return Ok(new { message = "Use /funds/download-tokens to get tokens" });
    }

    [HttpGet("exporttoexcel")]
    [ValidateDownloadToken("excel")]
    public async Task<IActionResult> ExportToExcel()
    {
        try
        {
            var funds = await _fundService.GetAllFundsAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Funds");

                // Header Section (rows 1-4)
                worksheet.Cells[1, 1, 1, 5].Merge = true;
                worksheet.Cells[1, 1].Value = "FUND SUMMARY REPORT";
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 16;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 1, 2, 5].Merge = true;
                worksheet.Cells[2, 1].Value = "Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[3, 1, 3, 5].Merge = true;
                worksheet.Cells[3, 1].Value = string.Empty;

                // Table Header Row (row 4)
                worksheet.Cells[4, 1].Value = "Fund Name";
                worksheet.Cells[4, 2].Value = "Ticker Code";
                worksheet.Cells[4, 3].Value = "NAV Price";
                worksheet.Cells[4, 4].Value = "Market Price";
                worksheet.Cells[4, 5].Value = "Hold In Trust";

                // Style the header row
                using (var range = worksheet.Cells[4, 1, 4, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Data Rows
                int row = 5;
                foreach (var fund in funds)
                {
                    worksheet.Cells[row, 1].Value = fund.FundName;
                    worksheet.Cells[row, 2].Value = fund.TickerCode;
                    worksheet.Cells[row, 3].Value = fund.NavPrice;
                    worksheet.Cells[row, 4].Value = fund.MarketPrice;
                    worksheet.Cells[row, 5].Value = fund.HoldInTrust;

                    // Center align the numeric values
                    worksheet.Cells[row, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Footer Section (starts after data)
                int footerStartRow = row + 2;

                worksheet.Cells[footerStartRow, 1, footerStartRow, 5].Merge = true;
                worksheet.Cells[footerStartRow, 1].Value = "END OF REPORT";
                worksheet.Cells[footerStartRow, 1].Style.Font.Bold = true;
                worksheet.Cells[footerStartRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[footerStartRow + 1, 1, footerStartRow + 1, 5].Merge = true;
                worksheet.Cells[footerStartRow + 1, 1].Value = $"Total Funds: {funds.Count}";
                worksheet.Cells[footerStartRow + 1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[footerStartRow + 2, 1, footerStartRow + 2, 5].Merge = true;
                worksheet.Cells[footerStartRow + 2, 1].Value = "This document is confidential and intended for internal use only.";
                worksheet.Cells[footerStartRow + 2, 1].Style.Font.Italic = true;
                worksheet.Cells[footerStartRow + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Add borders to the entire table
                var tableRange = worksheet.Cells[4, 1, row - 1, 5];
                tableRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                tableRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                tableRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                tableRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                var excelBytes = await package.GetAsByteArrayAsync();

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting funds to Excel: {Message}", ex.Message);
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("exporttocsv")]
    [ValidateDownloadToken("csv")]
    public async Task<IActionResult> ExportToCsv()
    {
        try
        {
            var funds = await _fundService.GetAllFundsAsync();

            // Use StringBuilder to build CSV content
            var csvBuilder = new StringBuilder();

            // Header Section
            csvBuilder.AppendLine("FUND SUMMARY REPORT");
            csvBuilder.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            csvBuilder.AppendLine();

            // Table Header Row
            csvBuilder.AppendLine("Fund Name,Ticker Code,NAV Price,Market Price,Hold In Trust");

            // Data Rows
            foreach (var fund in funds)
            {
                // Escape values that contain commas or quotes
                string fundName = EscapeCsvValue(fund.FundName);
                string tickerCode = EscapeCsvValue(fund.TickerCode);
                string navPrice = fund.NavPrice.ToString();
                string marketPrice = fund.MarketPrice.ToString();
                string holdInTrust = fund.HoldInTrust ?? "";

                csvBuilder.AppendLine($"{fundName},{tickerCode},{navPrice},{marketPrice},{holdInTrust}");
            }

            // Footer Section
            csvBuilder.AppendLine();
            csvBuilder.AppendLine("END OF REPORT");
            csvBuilder.AppendLine($"Total Funds: {funds.Count}");
            csvBuilder.AppendLine("This document is confidential and intended for internal use only.");

            var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            return File(
                csvBytes,
                "text/csv",
                $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting funds to CSV: {Message}", ex.Message);
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("exporttoexcel-free")]
    [ValidateDownloadToken("excel-free")]
    public async Task<IActionResult> ExportToExcelFree()
    {
        try
        {
            var funds = await _fundService.GetAllFundsAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Funds");

            // Header Section (rows 1-4)
            worksheet.Cell("A1").Value = "FUND SUMMARY REPORT";
            worksheet.Range("A1:E1").Merge();
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Font.FontSize = 16;
            worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell("A2").Value = "Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Range("A2:E2").Merge();
            worksheet.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Range("A3:E3").Merge();

            // Table Header Row (row 4)
            worksheet.Cell("A4").Value = "Fund Name";
            worksheet.Cell("B4").Value = "Ticker Code";
            worksheet.Cell("C4").Value = "NAV Price";
            worksheet.Cell("D4").Value = "Market Price";
            worksheet.Cell("E4").Value = "Hold In Trust";

            // Style the header row
            var headerRange = worksheet.Range("A4:E4");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Data Rows
            int row = 5;
            foreach (var fund in funds)
            {
                worksheet.Cell(row, 1).Value = fund.FundName;
                worksheet.Cell(row, 2).Value = fund.TickerCode;
                worksheet.Cell(row, 3).Value = fund.NavPrice;
                worksheet.Cell(row, 4).Value = fund.MarketPrice;
                worksheet.Cell(row, 5).Value = fund.HoldInTrust;

                // Center align the numeric values
                worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Add borders to data row
                var dataRange = worksheet.Range(row, 1, row, 5);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Footer Section (starts after data)
            int footerStartRow = row + 2;

            worksheet.Cell(footerStartRow, 1).Value = "END OF REPORT";
            worksheet.Range(footerStartRow, 1, footerStartRow, 5).Merge();
            worksheet.Cell(footerStartRow, 1).Style.Font.Bold = true;
            worksheet.Cell(footerStartRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(footerStartRow + 1, 1).Value = $"Total Funds: {funds.Count}";
            worksheet.Range(footerStartRow + 1, 1, footerStartRow + 1, 5).Merge();
            worksheet.Cell(footerStartRow + 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(footerStartRow + 2, 1).Value = "This document is confidential and intended for internal use only.";
            worksheet.Range(footerStartRow + 2, 1, footerStartRow + 2, 5).Merge();
            worksheet.Cell(footerStartRow + 2, 1).Style.Font.Italic = true;
            worksheet.Cell(footerStartRow + 2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var excelBytes = stream.ToArray();

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting funds to Excel (ClosedXML): {Message}", ex.Message);
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("exporttopdf")]
    [ValidateDownloadToken("pdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        try
        {
            var funds = await _fundService.GetAllFundsAsync();

            // Define columns for PDF export - other columns are compact to leave room for fund names
            var columns = new List<PdfColumnDefinition>
            {
                new() { PropertyName = "FundName", HeaderText = "Fund Name", Alignment = XStringAlignment.Near },
                new() { PropertyName = "TickerCode", HeaderText = "Ticker", Width = 70 },
                new() { PropertyName = "NavPrice", HeaderText = "NAV Price", Width = 60, Format = "F2", ShowAverage = true },
                new() { PropertyName = "MarketPrice", HeaderText = "Market Price", Width = 60, Format = "F2", ShowAverage = true },
                new() { PropertyName = "HoldInTrust", HeaderText = "Hold In Trust", Width = 60 }
            };

            // Configure export options
            var options = new PdfExportOptions
            {
                ReportTitle = "FUND SUMMARY REPORT\nThis report provides a summary of all funds including their NAV and market prices.\nwhat is more, it is generated using a custom PDF export service.\n financial report",
                ItemsPerPage = 50,
                Disclaimer = "FUND SUMMARY REPORT\nThis report provides a summary of all funds including their NAV and market prices.\nwhat is more, it is generated using a custom PDF export service.\n financial report",
                FooterText = "This document is confidential and intended for internal use only.",
                ShowAverageRow = true
            };

            var pdfBytes = _pdfExportService.ExportToPdf(funds, columns, options);

            return File(
                pdfBytes,
                "application/pdf",
                $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting funds to PDF: {Message}", ex.Message);
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("exporttoexcel-generic")]
    [ValidateDownloadToken("excel-generic")]
    public async Task<IActionResult> ExportToExcelGeneric()
    {
        try
        {
            var funds = await _fundService.GetAllFundsAsync();

            // Define columns for generic Excel export
            var columns = new List<ExcelColumnDefinition>
            {
                new() { PropertyName = "FundName", HeaderText = "Fund Name", Alignment = ExcelAlignment.Left },
                new() { PropertyName = "TickerCode", HeaderText = "Ticker", Width = 15, Alignment = ExcelAlignment.Left },
                new() { PropertyName = "NavPrice", HeaderText = "NAV Price", Width = 12, Format = "F2", Alignment = ExcelAlignment.Right },
                new() { PropertyName = "MarketPrice", HeaderText = "Market Price", Width = 12, Format = "F2", Alignment = ExcelAlignment.Right },
                new() { PropertyName = "HoldInTrust", HeaderText = "Hold In Trust", Width = 15, Alignment = ExcelAlignment.Left }
            };

            // Configure export options with full styling control
            var options = new ExcelExportOptions
            {
                WorksheetName = "Funds",
                ReportTitle = "**FUND SUMMARY REPORT**\nThis report provides a summary of all funds\nincluding their **NAV** and **Market Prices**.",
                Disclaimer = "**IMPORTANT DISCLAIMER:**\nPast performance is **not indicative** of future results.\nPlease consult with a **qualified financial advisor**.",
                // Styling - Title
                TitleFont = new ExcelFontStyle { FontSize = 16, Bold = true, FontColor = "#000000" },
                // Styling - Subtitle
                SubtitleFont = new ExcelFontStyle { FontSize = 11, Italic = true, FontColor = "#333333" },
                // Styling - Header
                HeaderFont = new ExcelFontStyle { Bold = true, BackgroundColor = "#D3D3D3", FontColor = "#000000" },
                // Styling - Data
                DataFont = new ExcelFontStyle { FontSize = 10, FontColor = "#000000" },
                // Styling - Footer
                FooterFont = new ExcelFontStyle { FontSize = 9, Italic = true, FontColor = "#666666" },
                // Styling - Disclaimer
                DisclaimerFont = new ExcelFontStyle { FontSize = 8, FontColor = "#999999" },
                // Other options
                AutoFitColumns = true,
                ShowBorders = true
            };

            var excelBytes = _excelExportService.ExportToExcel(funds, columns, options);

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting funds to Excel (generic): {Message}", ex.Message);
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    private string EscapeCsvValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        // If value contains comma, quote, or newline, wrap in quotes and escape quotes
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }

    [HttpGet("historical-nav-pdf/{fundId}")]
    [ValidateDownloadToken("pdf")]
    public async Task<IActionResult> ExportHistoricalNavToPdf(int fundId)
    {
        try
        {
            var viewModel = await _fundHistoricalNavService.GetHistoricalNavViewModelAsync(fundId);
            var historicalNavs = await _fundHistoricalNavService.GetHistoricalNavsByFundIdAsync(fundId);

            // Define columns for PDF export
            var columns = new List<PdfColumnDefinition>
            {
                new() { PropertyName = "NavDate", HeaderText = "Date", Width = 80, Format = "d" },
                new() { PropertyName = "NavPrice", HeaderText = "NAV Price", Width = 70, Format = "F2" },
                new() { PropertyName = "MarketPrice", HeaderText = "Market Price", Width = 70, Format = "F2" },
                new() { PropertyName = "DailyChangePercent", HeaderText = "Daily Change %", Width = 70, Format = "F2" },
                new() { PropertyName = "NetAssetValue", HeaderText = "Net Asset Value (M)", Width = 80, Format = "F2" }
            };

            var options = new PdfExportOptions
            {
                ReportTitle = $"HISTORICAL NAV VALUES\n{viewModel.FundName} ({viewModel.TickerCode})",
                ItemsPerPage = 50,
                Disclaimer = "This report contains historical NAV prices and is for informational purposes only.",
                FooterText = $"Fund: {viewModel.FundName} | Ticker: {viewModel.TickerCode}",
                ShowAverageRow = false
            };

            var pdfBytes = _pdfExportService.ExportToPdf(historicalNavs, columns, options);

            return File(
                pdfBytes,
                "application/pdf",
                $"HistoricalNav_{viewModel.TickerCode}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting historical NAV to PDF: {Message}", ex.Message);
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("historical-nav-excel/{fundId}")]
    [ValidateDownloadToken("excel-generic")]
    public async Task<IActionResult> ExportHistoricalNavToExcel(int fundId)
    {
        try
        {
            var viewModel = await _fundHistoricalNavService.GetHistoricalNavViewModelAsync(fundId);
            var historicalNavs = await _fundHistoricalNavService.GetHistoricalNavsByFundIdAsync(fundId);

            // Define columns for Excel export
            var columns = new List<ExcelColumnDefinition>
            {
                new() { PropertyName = "NavDate", HeaderText = "Date", Width = 15, Alignment = ExcelAlignment.Left },
                new() { PropertyName = "NavPrice", HeaderText = "NAV Price", Width = 12, Format = "F2", Alignment = ExcelAlignment.Left },
                new() { PropertyName = "MarketPrice", HeaderText = "Market Price", Width = 12, Format = "F2", Alignment = ExcelAlignment.Left },
                new() { PropertyName = "DailyChangePercent", HeaderText = "Daily Change %", Width = 12, Format = "F2", Alignment = ExcelAlignment.Left },
                new() { PropertyName = "NetAssetValue", HeaderText = "Net Asset Value (M)", Width = 15, Format = "F2", Alignment = ExcelAlignment.Left }
            };

            var options = new ExcelExportOptions
            {
                WorksheetName = "Historical NAV",
                ReportTitle = $"**HISTORICAL NAV VALUES**\n{viewModel.FundName} ({viewModel.TickerCode})",
                Disclaimer = "**Disclaimer:** This report contains historical NAV prices and is for informational purposes only.\nPast performance is not indicative of future results.",
                TitleFont = new ExcelFontStyle { FontSize = 14, Bold = true },
                HeaderFont = new ExcelFontStyle { Bold = true, BackgroundColor = "#D3D3D3" },
                DataFont = new ExcelFontStyle { FontSize = 10 },
                AutoFitColumns = true,
                ShowBorders = true
            };

            var excelBytes = _excelExportService.ExportToExcel(historicalNavs, columns, options);

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"HistoricalNav_{viewModel.TickerCode}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting historical NAV to Excel: {Message}", ex.Message);
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
