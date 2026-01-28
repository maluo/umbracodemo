using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Text;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco13.Services;

namespace Umbraco13.Controllers;

[Route("funds")]
public class FundsController : UmbracoController
{
    private readonly IFundService _fundService;
    private readonly ILogger<FundsController> _logger;

    public FundsController(IFundService fundService, ILogger<FundsController> logger)
    {
        _fundService = fundService;
        _logger = logger;
    }

    [HttpGet("update-table")]
    public async Task<IActionResult> UpdateTable()
    {
        // Return the ViewComponent result
        return ViewComponent("FundsTable");
    }

    [HttpGet("exporttoexcel")]
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
            _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
            if (ex.InnerException != null)
            {
                _logger.LogError(ex, "Inner exception: {InnerMessage}", ex.InnerException.Message);
            }
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("exporttocsv")]
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

            // Footer Section (span all 5 columns)
            csvBuilder.AppendLine();
            csvBuilder.AppendLine("END OF REPORT,,,,");
            csvBuilder.AppendLine($"Total Funds: {funds.Count},,,,");
            csvBuilder.AppendLine("This document is confidential and intended for internal use only.,,,,");

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
            _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
            if (ex.InnerException != null)
            {
                _logger.LogError(ex, "Inner exception: {InnerMessage}", ex.InnerException.Message);
            }
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
}
