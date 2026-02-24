using System.Reflection;
using System.Text;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp.Pdf;

namespace Umbraco13.Services;

/// <summary>
/// Defines a column for HTML PDF table export
/// </summary>
public class HtmlPdfColumnDefinition
{
    public string PropertyName { get; set; } = string.Empty;
    public string HeaderText { get; set; } = string.Empty;
    public string? Width { get; set; }
    public string? Alignment { get; set; } = "left";
    public string? Format { get; set; }
    public Func<object?, string>? CustomFormatter { get; set; }
}

/// <summary>
/// Options for HTML-based PDF export
/// </summary>
public class HtmlPdfExportOptions
{
    public string ReportTitle { get; set; } = "Report";
    public string? Subtitle { get; set; }
    public string? Disclaimer { get; set; }
    public string? FooterText { get; set; }
    public string BorderWidth { get; set; } = "1px";
    public string BorderColor { get; set; } = "#333";
    public string HeaderBackgroundColor { get; set; } = "#f2f2f2";
    public string HeaderFontSize { get; set; } = "14px";
    public string DataFontSize { get; set; } = "12px";
    public string TitleFontSize { get; set; } = "24px";
}

/// <summary>
/// Interface for HTML-based PDF print service
/// </summary>
public interface IHtmlPdfPrintService
{
    byte[] ExportToPdf<T>(
        IEnumerable<T> data,
        IList<HtmlPdfColumnDefinition> columns,
        HtmlPdfExportOptions? options = null);
}

/// <summary>
/// Service to generate HTML tables and convert to PDF using HtmlRenderer.PdfSharp
/// </summary>
public class HtmlPdfPrintService : IHtmlPdfPrintService
{
    private readonly ILogger<HtmlPdfPrintService> _logger;

    public HtmlPdfPrintService(ILogger<HtmlPdfPrintService> logger)
    {
        _logger = logger;
    }

    public byte[] ExportToPdf<T>(IEnumerable<T> data, IList<HtmlPdfColumnDefinition> columns, HtmlPdfExportOptions? options = null)
    {
        options ??= new HtmlPdfExportOptions();
        var dataList = data.ToList();

        var html = GenerateHtmlTable(dataList, columns, options);
        var pdfBytes = ConvertHtmlToPdf(html);

        return pdfBytes;
    }

    private string GenerateHtmlTable<T>(List<T> data, IList<HtmlPdfColumnDefinition> columns, HtmlPdfExportOptions options)
    {
        var html = new StringBuilder();

        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset=\"UTF-8\">");
        html.AppendLine("<title>" + System.Net.WebUtility.HtmlEncode(options.ReportTitle) + "</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
        html.AppendLine("h1 { color: #333; margin-bottom: 10px; }");
        html.AppendLine("h2 { color: #666; margin-bottom: 20px; font-size: 14px; }");
        html.AppendLine("table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
        html.AppendLine("th, td { border: " + options.BorderWidth + " solid " + options.BorderColor + "; padding: 8px; text-align: left; }");
        html.AppendLine("th { background-color: " + options.HeaderBackgroundColor + "; font-weight: bold; font-size: " + options.HeaderFontSize + "; }");
        html.AppendLine("td { font-size: " + options.DataFontSize + "; }");
        html.AppendLine("footer { margin-top: 30px; text-align: center; font-size: 10px; color: #666; }");
        html.AppendLine(".disclaimer { margin-top: 20px; font-size: 9px; color: #999; }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        html.AppendLine("<h1>" + System.Net.WebUtility.HtmlEncode(options.ReportTitle).Replace("\n", "<br>") + "</h1>");

        if (!string.IsNullOrEmpty(options.Subtitle))
        {
            html.AppendLine("<h2>" + System.Net.WebUtility.HtmlEncode(options.Subtitle).Replace("\n", "<br>") + "</h2>");
        }

        html.AppendLine("<table>");

        // Header row
        html.Append("  <tr>");
        foreach (var column in columns)
        {
            var width = !string.IsNullOrEmpty(column.Width) ? " width=\"" + column.Width + "\"" : "";
            html.Append("<th style=\"text-align: " + column.Alignment + ";\"" + width + ">" + System.Net.WebUtility.HtmlEncode(column.HeaderText) + "</th>");
        }
        html.AppendLine("</tr>");

        // Data rows
        foreach (var item in data)
        {
            html.Append("  <tr>");
            foreach (var column in columns)
            {
                var value = GetPropertyValue(item, column.PropertyName);
                var text = FormatValue(value, column);
                html.Append("<td style=\"text-align: " + column.Alignment + ";\">" + System.Net.WebUtility.HtmlEncode(text) + "</td>");
            }
            html.AppendLine("</tr>");
        }

        html.AppendLine("</table>");

        if (!string.IsNullOrEmpty(options.FooterText))
        {
            html.AppendLine("<footer>" + System.Net.WebUtility.HtmlEncode(options.FooterText).Replace("\n", "<br>") + "</footer>");
        }

        if (!string.IsNullOrEmpty(options.Disclaimer))
        {
            html.AppendLine("<div class=\"disclaimer\">" + System.Net.WebUtility.HtmlEncode(options.Disclaimer).Replace("\n", "<br>") + "</div>");
        }

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    private byte[] ConvertHtmlToPdf(string html)
    {
        try
        {
            // Use HtmlRenderer.PdfSharp to convert HTML to PDF
            var pdf = PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);

            using var stream = new MemoryStream();
            pdf.Save(stream, false);
            return stream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting HTML to PDF: {Message}", ex.Message);
            throw;
        }
    }

    private object? GetPropertyValue<T>(T item, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        return property?.GetValue(item);
    }

    private string FormatValue(object? value, HtmlPdfColumnDefinition column)
    {
        if (value == null)
            return "";

        if (column.CustomFormatter != null)
            return column.CustomFormatter(value);

        if (!string.IsNullOrEmpty(column.Format))
        {
            try
            {
                return string.Format("{0:" + column.Format + "}", value);
            }
            catch
            {
                return value.ToString() ?? "";
            }
        }

        return value.ToString() ?? "";
    }
}
