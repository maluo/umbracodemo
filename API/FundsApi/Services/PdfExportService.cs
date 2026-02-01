using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using FundsApi.Models.ExportModels;

namespace FundsApi.Services;

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
    /// Draws text with support for **bold** markup. Use **text** for bold portions.
    /// </summary>
    private void DrawFormattedText(
        XGraphics gfx,
        string text,
        double xPos,
        double yPos,
        double maxWidth,
        double lineHeight,
        XFont font,
        XFont fontBold,
        XBrush brush,
        XStringAlignment alignment = XStringAlignment.Center)
    {
        // Parse **bold** markers
        var segments = new List<(string text, bool isBold)>();
        int currentIndex = 0;

        while (currentIndex < text.Length)
        {
            int boldStart = text.IndexOf("**", currentIndex);

            if (boldStart == -1)
            {
                // No more bold markers, add remaining text as regular
                segments.Add((text.Substring(currentIndex), false));
                break;
            }
            else if (boldStart > currentIndex)
            {
                // Add regular text before bold marker
                segments.Add((text.Substring(currentIndex, boldStart - currentIndex), false));
            }

            // Find closing **
            int boldEnd = text.IndexOf("**", boldStart + 2);
            if (boldEnd == -1)
            {
                // Unclosed bold marker, treat as regular text
                segments.Add((text.Substring(currentIndex), false));
                break;
            }

            // Add bold text (without the ** markers)
            segments.Add((text.Substring(boldStart + 2, boldEnd - boldStart - 2), true));
            currentIndex = boldEnd + 2;
        }

        // Calculate total width for alignment
        double totalWidth = 0;
        foreach (var (segText, isBold) in segments)
        {
            var segFont = isBold ? fontBold : font;
            totalWidth += gfx.MeasureString(segText, segFont).Width;
        }

        // Calculate starting X position based on alignment
        double currentX = xPos;
        if (alignment == XStringAlignment.Center)
        {
            currentX = xPos + (maxWidth - totalWidth) / 2;
        }
        else if (alignment == XStringAlignment.Far)
        {
            currentX = xPos + maxWidth - totalWidth;
        }

        // Draw each segment
        foreach (var (segText, isBold) in segments)
        {
            if (string.IsNullOrEmpty(segText)) continue;

            var segFont = isBold ? fontBold : font;
            gfx.DrawString(segText, segFont, brush,
                new XRect(currentX, yPos, maxWidth, lineHeight), XStringFormats.TopLeft);
            currentX += gfx.MeasureString(segText, segFont).Width;
        }
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

        // Draw first page header and calculate where table should start
        double headerHeight = DrawPageHeader(page, gfx, currentPageNum, options, font, fontHeader);
        // Calculate table position: header end + minimal spacing (15 units)
        var firstPageTableTop = headerHeight + 15;
        DrawTableHeader(gfx, firstPageTableTop, columnWidths, columns, fontBold, options);

        // Data Rows
        double yPos = firstPageTableTop + options.RowHeight;

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

                // Draw continuation page header (just "Continued" message)
                DrawContinuationPageHeader(page, gfx, options, font);

                // Table starts closer to top on continuation pages
                yPos = 50; // Minimal spacing for continuation pages
                DrawTableHeader(gfx, yPos, columnWidths, columns, fontBold, options);
                yPos += options.RowHeight;
            }

            // Draw data row
            DrawDataRow(gfx, item, yPos, columnWidths, columns, font, options);
            yPos += options.RowHeight;
            itemsOnCurrentPage++;
        }

        // Draw average row (if enabled)
        if (options.ShowAverageRow)
        {
            yPos = DrawAverageRow(gfx, yPos, dataList, columnWidths, columns, font, fontBold, options);
        }

        // Draw disclaimer immediately after last table row (if provided)
        if (!string.IsNullOrEmpty(options.Disclaimer))
        {
            yPos = DrawDisclaimer(gfx, yPos, page, options, fontFooter, fontBold);
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
    /// Draw first page header (report title and subtitle)
    /// Returns the Y position after the header (header bottom edge)
    /// </summary>
    private double DrawPageHeader(PdfPage page, XGraphics gfx, int pageNum, PdfExportOptions options, XFont font, XFont fontHeader)
    {
        // Only first page gets full header
        if (pageNum != 1)
            return 20; // Return minimal position for non-first pages

        // First page - full title and subtitle (left aligned)
        double yPos = 30;

        // Support multi-line title with **bold** markup
        var titleLines = options.ReportTitle.Split('\n');
        foreach (var line in titleLines)
        {
            DrawFormattedText(gfx, line.Trim(), options.MarginLeft, yPos, page.Width - options.MarginLeft, 20, font, fontHeader, XBrushes.Black, XStringAlignment.Near);
            yPos += 20;
        }

        // Support multi-line subtitle with **bold** markup
        if (!string.IsNullOrEmpty(options.Subtitle))
        {
            yPos += 10; // Add spacing between title and subtitle
            var subtitleLines = options.Subtitle.Split('\n');
            foreach (var line in subtitleLines)
            {
                DrawFormattedText(gfx, line.Trim(), options.MarginLeft, yPos, page.Width - options.MarginLeft, 15, font, fontHeader, XBrushes.Black, XStringAlignment.Near);
                yPos += 15;
            }
        }

        return yPos; // Return the Y position after all header content
    }

    /// <summary>
    /// Draw continuation page header (just "(Continued)" message centered)
    /// </summary>
    private void DrawContinuationPageHeader(PdfPage page, XGraphics gfx, PdfExportOptions options, XFont font)
    {
        // Simple centered "(Continued)" message
        gfx.DrawString("(Continued)", new XFont(options.FontFamily, 12, XFontStyleEx.Bold), XBrushes.DarkGray,
            new XRect(0, 20, page.Width, 20), XStringFormats.TopCenter);
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
    /// Draw average row with grey background
    /// </summary>
    private double DrawAverageRow<T>(
        XGraphics gfx,
        double yPos,
        List<T> dataList,
        List<double> columnWidths,
        IList<PdfColumnDefinition> columns,
        XFont font,
        XFont fontBold,
        PdfExportOptions options)
    {
        var pen = new XPen(XColors.Gray, 0.5);
        var brushGray = new XSolidBrush(XColor.FromArgb(240, 240, 240));

        double xPos = options.MarginLeft;

        for (int i = 0; i < columns.Count; i++)
        {
            var width = columnWidths[i];
            var column = columns[i];

            // Draw grey background for the cell
            gfx.DrawRectangle(pen, brushGray, xPos, yPos, width, options.RowHeight);

            if (i == 0)
            {
                // First column - show "Average" label
                gfx.DrawString(options.AverageRowLabel, fontBold, XBrushes.Black,
                    new XRect(xPos + 5, yPos, width - 5, options.RowHeight), XStringFormats.CenterLeft);
            }
            else if (column.ShowAverage)
            {
                // Calculate average for numeric columns
                double sum = 0;
                int count = 0;

                foreach (var item in dataList)
                {
                    var value = GetPropertyValue(item, column.PropertyName);
                    if (value != null && IsNumeric(value))
                    {
                        sum += Convert.ToDouble(value);
                        count++;
                    }
                }

                string avgText = "";
                if (count > 0)
                {
                    var avg = sum / count;

                    // Use format if provided, otherwise show 2 decimal places
                    if (!string.IsNullOrEmpty(column.Format))
                    {
                        avgText = string.Format("{0:" + column.Format + "}", avg);
                    }
                    else if (column.CustomFormatter != null)
                    {
                        avgText = column.CustomFormatter(avg);
                    }
                    else
                    {
                        avgText = avg.ToString("F2");
                    }
                }

                // Draw average value (centered)
                gfx.DrawString(avgText, fontBold, XBrushes.Black,
                    new XRect(xPos, yPos, width, options.RowHeight), XStringFormats.Center);
            }

            xPos += width;
        }

        return yPos + options.RowHeight;
    }

    /// <summary>
    /// Check if a value is numeric
    /// </summary>
    private bool IsNumeric(object value)
    {
        return value is int or double or decimal or float or long or short or byte or uint or ulong or ushort or sbyte;
    }

    /// <summary>
    /// Draw disclaimer immediately after table rows (before footer)
    /// </summary>
    private double DrawDisclaimer(
        XGraphics gfx,
        double yPos,
        PdfPage page,
        PdfExportOptions options,
        XFont fontFooter,
        XFont fontBold)
    {
        // Add spacing after last table row
        yPos += 15;

        // Draw disclaimer separator line (full width like table)
        gfx.DrawLine(new XPen(XColors.Gray, 0.5), options.MarginLeft, yPos, page.Width - options.MarginLeft, yPos);
        yPos += 10;

        // Draw disclaimer text (support multi-line with \n and **bold** markup), left aligned
        var disclaimerLines = options.Disclaimer!.Split('\n');
        foreach (var line in disclaimerLines)
        {
            DrawFormattedText(gfx, line.Trim(), options.MarginLeft, yPos, page.Width - options.MarginLeft, 12, fontFooter, fontBold, XBrushes.Black, XStringAlignment.Near);
            yPos += 12;
        }

        return yPos;
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
            gfx.DrawString($"Showing {startItem}-{endItem} of {totalCount} records", fontPageNum, XBrushes.DarkGray,
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

        // Footer text (support multi-line with \n and **bold** markup)
        footerY += 40;
        var footerText = options.FooterText ?? "This document is confidential and intended for internal use only.";
        var footerLines = footerText.Split('\n');
        foreach (var line in footerLines)
        {
            DrawFormattedText(gfx, line.Trim(), 0, footerY, page.Width, 12, fontFooter, fontBold, XBrushes.Black);
            footerY += 12;
        }

        // Page info below the report footer
        footerY = page.Height - options.MarginBottom;
        var pageInfo = $"Page {pageNum} | Showing {startItem}-{totalCount} of {totalCount} records | {DateTime.Now:yyyy-MM-dd}";
        gfx.DrawString(pageInfo, fontPageNum, XBrushes.DarkGray,
            new XRect(0, footerY, page.Width, 15), XStringFormats.TopCenter);
    }
}
