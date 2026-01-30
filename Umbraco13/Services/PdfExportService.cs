using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Umbraco13.Services;

/// <summary>
/// Defines a column for PDF export
/// </summary>
public class PdfColumnDefinition
{
    /// <summary>
    /// Property name in the data object (case-insensitive)
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Header text to display for this column
    /// </summary>
    public string HeaderText { get; set; } = string.Empty;

    /// <summary>
    /// Fixed width for this column (in PDF units). If 0, will be calculated automatically.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Optional format string for formatting values (e.g., "C2" for currency)
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Optional custom formatter function
    /// </summary>
    public Func<object?, string>? CustomFormatter { get; set; }

    /// <summary>
    /// Alignment for this column (default: Center)
    /// </summary>
    public XStringAlignment Alignment { get; set; } = XStringAlignment.Center;
}

/// <summary>
/// Options for PDF export
/// </summary>
public class PdfExportOptions
{
    /// <summary>
    /// Report title displayed on the first page
    /// </summary>
    public string ReportTitle { get; set; } = "Report";

    /// <summary>
    /// Optional subtitle displayed below the title on the first page
    /// </summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// Page size (default: A4)
    /// </summary>
    public PageSize PageSize { get; set; } = PageSize.A4;

    /// <summary>
    /// Left margin in PDF units (default: 50)
    /// </summary>
    public double MarginLeft { get; set; } = 50;

    /// <summary>
    /// Top margin in PDF units (default: 100)
    /// </summary>
    public double MarginTop { get; set; } = 100;

    /// <summary>
    /// Bottom margin in PDF units (default: 50)
    /// </summary>
    public double MarginBottom { get; set; } = 50;

    /// <summary>
    /// Row height in PDF units (default: 25)
    /// </summary>
    public double RowHeight { get; set; } = 25;

    /// <summary>
    /// Items per page (default: 25)
    /// </summary>
    public int ItemsPerPage { get; set; } = 25;

    /// <summary>
    /// Font family name (default: Arial)
    /// </summary>
    public string FontFamily { get; set; } = "Arial";

    /// <summary>
    /// Font size for regular text (default: 10)
    /// </summary>
    public double FontSize { get; set; } = 10;

    /// <summary>
    /// Font size for headers (default: 10, Bold)
    /// </summary>
    public double HeaderFontSize { get; set; } = 10;

    /// <summary>
    /// Font size for report title (default: 20, Bold)
    /// </summary>
    public double TitleFontSize { get; set; } = 20;

    /// <summary>
    /// Font size for page numbers (default: 9)
    /// </summary>
    public double PageNumberFontSize { get; set; } = 9;

    /// <summary>
    /// Font size for footer text (default: 8)
    /// </summary>
    public double FooterFontSize { get; set; } = 8;

    /// <summary>
    /// Show page numbers (default: true)
    /// </summary>
    public bool ShowPageNumbers { get; set; } = true;

    /// <summary>
    /// Show "Showing X-Y of Z" indicator (default: true)
    /// </summary>
    public bool ShowProgressIndicator { get; set; } = true;

    /// <summary>
    /// Show "END OF REPORT" footer on last page (default: true)
    /// </summary>
    public bool ShowReportFooter { get; set; } = true;

    /// <summary>
    /// Footer text for "END OF REPORT" section (default: "This document is confidential...")
    /// </summary>
    public string? FooterText { get; set; }

    /// <summary>
    /// Optional callback to get the total count (useful when passing a paged list)
    /// </summary>
    public Func<int>? GetTotalCount { get; set; }
}

/// <summary>
/// Generic PDF export service for exporting any list of objects to PDF
/// </summary>
public class PdfExportService : IPdfExportService
{
    private readonly ILogger<PdfExportService> _logger;

    public PdfExportService(ILogger<PdfExportService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Export a list of objects to PDF
    /// </summary>
    public byte[] ExportToPdf<T>(
        IEnumerable<T> data,
        IList<PdfColumnDefinition> columns,
        PdfExportOptions? options = null)
    {
        options ??= new PdfExportOptions();
        var dataList = data.ToList();
        var totalCount = options.GetTotalCount?.Invoke() ?? dataList.Count;

        // Create PDF document
        var document = new PdfDocument();
        var page = document.AddPage();
        page.Size = options.PageSize;

        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont(options.FontFamily, options.FontSize);
        var fontBold = new XFont(options.FontFamily, options.HeaderFontSize, XFontStyleEx.Bold);
        var fontHeader = new XFont(options.FontFamily, options.TitleFontSize, XFontStyleEx.Bold);
        var fontFooter = new XFont(options.FontFamily, options.FooterFontSize);
        var fontPageNum = new XFont(options.FontFamily, options.PageNumberFontSize);

        // Calculate column widths
        var columnWidths = CalculateColumnWidths(gfx, dataList, columns, page, options, font, fontBold);

        var currentPageNum = 1;
        var itemsOnCurrentPage = 0;
        var firstItemOnPage = 1;

        // Draw first page header and table header
        DrawPageHeader(page, gfx, currentPageNum, options, font, fontHeader);
        DrawTableHeader(gfx, options.MarginTop, columnWidths, columns, fontBold, options);

        // Data Rows
        double yPos = options.MarginTop + options.RowHeight;

        for (int i = 0; i < dataList.Count; i++)
        {
            var item = dataList[i];

            // Check if we need a new page
            if (itemsOnCurrentPage >= options.ItemsPerPage || yPos + options.RowHeight > page.Height - options.MarginBottom - 50)
            {
                // Draw footer for current page
                DrawPageFooter(page, gfx, currentPageNum, firstItemOnPage, i, totalCount, fontPageNum, options);

                // Create new page
                page = document.AddPage();
                page.Size = options.PageSize;
                gfx = XGraphics.FromPdfPage(page);
                currentPageNum++;
                firstItemOnPage = i + 1;
                itemsOnCurrentPage = 0;
                yPos = options.MarginTop;

                // Draw header and table header on new page
                DrawPageHeader(page, gfx, currentPageNum, options, font, fontHeader);
                DrawTableHeader(gfx, yPos, columnWidths, columns, fontBold, options);
                yPos += options.RowHeight;
            }

            // Draw data row
            DrawDataRow(gfx, item, yPos, columnWidths, columns, font, options);
            yPos += options.RowHeight;
            itemsOnCurrentPage++;
        }

        // Draw report footer on last page
        DrawReportFooter(page, gfx, currentPageNum, firstItemOnPage, totalCount, font, fontBold, fontFooter, fontPageNum, options);

        // Save PDF to memory stream
        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }

    /// <summary>
    /// Calculate column widths, either using fixed widths or auto-calculating based on content
    /// </summary>
    private List<double> CalculateColumnWidths<T>(
        XGraphics gfx,
        List<T> data,
        IList<PdfColumnDefinition> columns,
        PdfPage page,
        PdfExportOptions options,
        XFont font,
        XFont fontBold)
    {
        var widths = new List<double>();
        var totalFixedWidth = 0.0;
        var autoWidthColumns = new List<int>();

        // First pass: identify fixed vs auto columns
        for (int i = 0; i < columns.Count; i++)
        {
            if (columns[i].Width > 0)
            {
                widths.Add(columns[i].Width);
                totalFixedWidth += columns[i].Width;
            }
            else
            {
                widths.Add(0);
                autoWidthColumns.Add(i);
            }
        }

        // Calculate available width for auto-sized columns
        double maxTableWidth = page.Width - (options.MarginLeft * 2);
        double remainingWidth = maxTableWidth - totalFixedWidth;

        if (autoWidthColumns.Count == 0)
        {
            return widths;
        }

        // Calculate auto widths based on content
        var autoWidths = new Dictionary<int, double>();

        foreach (var colIndex in autoWidthColumns)
        {
            var column = columns[colIndex];
            double maxWidth = gfx.MeasureString(column.HeaderText, fontBold).Width;

            // Check data width for this column
            foreach (var item in data)
            {
                var value = GetPropertyValue(item, column.PropertyName);
                var text = FormatValue(value, column);
                var textWidth = gfx.MeasureString(text, font).Width;
                if (textWidth > maxWidth)
                {
                    maxWidth = textWidth;
                }
            }

            // Add padding
            autoWidths[colIndex] = maxWidth + 20; // 10 units padding on each side
            // Ensure minimum width of 80
            if (autoWidths[colIndex] < 80)
            {
                autoWidths[colIndex] = 80;
            }
        }

        // Check if total exceeds available width
        double totalAutoWidth = autoWidths.Values.Sum();

        if (totalAutoWidth > remainingWidth)
        {
            // Scale down proportionally
            double scale = remainingWidth / totalAutoWidth;
            foreach (var colIndex in autoWidthColumns)
            {
                autoWidths[colIndex] *= scale;
                _logger.LogWarning("PDF Export: Column '{ColumnName}' width scaled from {OriginalWidth} to {ScaledWidth}",
                    columns[colIndex].PropertyName, autoWidths[colIndex] / scale, autoWidths[colIndex]);
            }
        }

        // Assign calculated widths
        for (int i = 0; i < columns.Count; i++)
        {
            if (autoWidthColumns.Contains(i))
            {
                widths[i] = autoWidths[i];
            }
        }

        return widths;
    }

    /// <summary>
    /// Get property value from object using reflection
    /// </summary>
    private object? GetPropertyValue<T>(T item, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        return property?.GetValue(item);
    }

    /// <summary>
    /// Format value for display
    /// </summary>
    private string FormatValue(object? value, PdfColumnDefinition column)
    {
        if (value == null)
            return "";

        // Use custom formatter if provided
        if (column.CustomFormatter != null)
            return column.CustomFormatter(value);

        // Use format string if provided
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

    /// <summary>
    /// Draw page header (report title)
    /// </summary>
    private void DrawPageHeader(PdfPage page, XGraphics gfx, int pageNum, PdfExportOptions options, XFont font, XFont fontHeader)
    {
        if (pageNum == 1)
        {
            // First page - full title and subtitle
            gfx.DrawString(options.ReportTitle, fontHeader, XBrushes.Black,
                new XRect(0, 30, page.Width, 40), XStringFormats.TopCenter);

            if (!string.IsNullOrEmpty(options.Subtitle))
            {
                gfx.DrawString(options.Subtitle, font, XBrushes.Black,
                    new XRect(0, 65, page.Width, 20), XStringFormats.TopCenter);
            }
        }
        else
        {
            // Continuation pages - smaller title
            gfx.DrawString($"{options.ReportTitle} (Continued)", new XFont(options.FontFamily, 14, XFontStyleEx.Bold), XBrushes.Black,
                new XRect(0, 30, page.Width, 30), XStringFormats.TopCenter);
        }
    }

    /// <summary>
    /// Draw table header row
    /// </summary>
    private void DrawTableHeader(
        XGraphics gfx,
        double yPos,
        List<double> columnWidths,
        IList<PdfColumnDefinition> columns,
        XFont fontBold,
        PdfExportOptions options)
    {
        var pen = new XPen(XColors.Gray, 0.5);
        var brushHeader = new XSolidBrush(XColor.FromArgb(224, 224, 224));

        double xPos = options.MarginLeft;

        for (int i = 0; i < columns.Count; i++)
        {
            var width = columnWidths[i];
            gfx.DrawRectangle(pen, brushHeader, xPos, yPos, width, options.RowHeight);
            gfx.DrawString(columns[i].HeaderText, fontBold, XBrushes.Black,
                new XRect(xPos, yPos, width, options.RowHeight), XStringFormats.Center);
            xPos += width;
        }
    }

    /// <summary>
    /// Draw a data row
    /// </summary>
    private void DrawDataRow<T>(
        XGraphics gfx,
        T item,
        double yPos,
        List<double> columnWidths,
        IList<PdfColumnDefinition> columns,
        XFont font,
        PdfExportOptions options)
    {
        var pen = new XPen(XColors.Gray, 0.5);
        var brushWhite = new XSolidBrush(XColors.White);

        double xPos = options.MarginLeft;

        for (int i = 0; i < columns.Count; i++)
        {
            var width = columnWidths[i];
            var value = GetPropertyValue(item, columns[i].PropertyName);
            var text = FormatValue(value, columns[i]);

            gfx.DrawRectangle(pen, brushWhite, xPos, yPos, width, options.RowHeight);

            var format = columns[i].Alignment;
            if (format == XStringAlignment.Center)
            {
                gfx.DrawString(text, font, XBrushes.Black,
                    new XRect(xPos, yPos, width, options.RowHeight), XStringFormats.Center);
            }
            else if (format == XStringAlignment.Near)
            {
                // For left-aligned text, add 5 units left padding but keep full width
                gfx.DrawString(text, font, XBrushes.Black,
                    new XRect(xPos + 5, yPos, width - 5, options.RowHeight), XStringFormats.CenterLeft);
            }
            else // Far
            {
                gfx.DrawString(text, font, XBrushes.Black,
                    new XRect(xPos, yPos, width - 5, options.RowHeight), XStringFormats.CenterRight);
            }

            xPos += width;
        }
    }

    /// <summary>
    /// Draw page footer (page number, progress indicator)
    /// </summary>
    private void DrawPageFooter(
        PdfPage page,
        XGraphics gfx,
        int pageNum,
        int startItem,
        int endItem,
        int totalCount,
        XFont fontPageNum,
        PdfExportOptions options)
    {
        double footerY = page.Height - options.MarginBottom;
        var totalPageWidth = page.Width;

        if (options.ShowPageNumbers)
        {
            gfx.DrawString($"Page {pageNum}", fontPageNum, XBrushes.DarkGray,
                new XRect(options.MarginLeft, footerY, 100, 15), XStringFormats.TopLeft);
        }

        if (options.ShowProgressIndicator)
        {
            gfx.DrawString($"Showing {startItem}-{endItem} of {totalCount} items", fontPageNum, XBrushes.DarkGray,
                new XRect(0, footerY, totalPageWidth, 15), XStringFormats.TopCenter);
        }

        // Date on right
        gfx.DrawString($"{DateTime.Now:yyyy-MM-dd}", fontPageNum, XBrushes.DarkGray,
            new XRect(totalPageWidth - options.MarginLeft - 100, footerY, 100, 15), XStringFormats.TopRight);
    }

    /// <summary>
    /// Draw report footer on last page
    /// </summary>
    private void DrawReportFooter(
        PdfPage page,
        XGraphics gfx,
        int pageNum,
        int startItem,
        int totalCount,
        XFont font,
        XFont fontBold,
        XFont fontFooter,
        XFont fontPageNum,
        PdfExportOptions options)
    {
        if (!options.ShowReportFooter)
            return;

        double footerY = page.Height - options.MarginBottom - 80;

        // Line separator
        gfx.DrawLine(new XPen(XColors.Gray, 1), options.MarginLeft, footerY, page.Width - options.MarginLeft, footerY);

        // End of report text
        footerY += 10;
        gfx.DrawString("END OF REPORT", fontBold, XBrushes.Black,
            new XRect(0, footerY, page.Width, 20), XStringFormats.TopCenter);
        gfx.DrawString($"Total Items: {totalCount}", font, XBrushes.Black,
            new XRect(0, footerY + 20, page.Width, 15), XStringFormats.TopCenter);

        var footerText = options.FooterText ?? "This document is confidential and intended for internal use only.";
        gfx.DrawString(footerText, fontFooter, XBrushes.Black,
            new XRect(0, footerY + 35, page.Width, 15), XStringFormats.TopCenter);

        // Page info below the report footer
        footerY += 60;
        var pageInfo = $"Page {pageNum} | Showing {startItem}-{totalCount} of {totalCount} items | {DateTime.Now:yyyy-MM-dd}";
        gfx.DrawString(pageInfo, fontPageNum, XBrushes.DarkGray,
            new XRect(0, footerY, page.Width, 15), XStringFormats.TopCenter);
    }
}
