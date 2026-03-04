using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Reflection;

namespace PdfTester;

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

    /// <summary>
    /// Alignment for the header text of this column (default: Center). If not set, uses Alignment value.
    /// </summary>
    public XStringAlignment? HeaderAlignment { get; set; }

    /// <summary>
    /// Calculate and show average for this column in summary row (only for numeric types)
    /// </summary>
    public bool ShowAverage { get; set; }
}

/// <summary>
/// Font styling options for PDF export
/// </summary>
public class PdfFontStyle
{
    /// <summary>
    /// Font family name (default: "Arial")
    /// </summary>
    public string FontFamily { get; set; } = "Arial";

    /// <summary>
    /// Font size in points (default: 10)
    /// </summary>
    public double FontSize { get; set; } = 10;

    /// <summary>
    /// Bold text (default: false)
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Italic text (default: false)
    /// </summary>
    public bool Italic { get; set; }
}

/// <summary>
/// Defines a custom last row for PDF export
/// </summary>
public class PdfLastRowDefinition
{
    /// <summary>
    /// Cell values for the last row (index maps to column position)
    /// </summary>
    public List<string> CellValues { get; set; } = new();

    /// <summary>
    /// Optional custom font style for the last row
    /// </summary>
    public PdfFontStyle? FontStyle { get; set; }
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
    /// Multi-line header text displayed at the top of the first page
    /// </summary>
    public List<string> HeaderLines { get; set; } = new();

    /// <summary>
    /// Multi-line disclaimer text displayed at the bottom of the last page
    /// </summary>
    public List<string> DisclaimerLines { get; set; } = new();

    /// <summary>
    /// Document title for PDF metadata (shows in PDF viewer title bar).
    /// If not specified, defaults to ReportTitle.
    /// </summary>
    public string? DocumentTitle { get; set; }

    /// <summary>
    /// Document author for PDF metadata.
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Document description/subject for PDF metadata.
    /// </summary>
    public string? Description { get; set; }

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
    /// Row height in PDF units (default: 18 - Excel-like)
    /// </summary>
    public double RowHeight { get; set; } = 18;
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
    /// Font size for report title (default: 16, Bold - H3 equivalent)
    /// </summary>
    public double TitleFontSize { get; set; } = 16;
    /// <summary>
    /// Font size for page numbers (default: 9)
    /// </summary>
    public double PageNumberFontSize { get; set; } = 9;

    /// <summary>
    /// Font size for footer text (default: 8)
    /// </summary>
    public double FooterFontSize { get; set; } = 8;

    /// <summary>
    /// Optional custom last row with user-provided values
    /// </summary>
    public PdfLastRowDefinition? LastRow { get; set; }

    /// <summary>
    /// Optional callback to get the total count (useful when passing a paged list)
    /// </summary>
    public Func<int>? GetTotalCount { get; set; }

    /// <summary>
    /// Show average row at end of table (default: false)
    /// </summary>
    public bool ShowAverageRow { get; set; }

    /// <summary>
    /// Label for average row (default: "Average")
    /// </summary>
    public string AverageRowLabel { get; set; } = "Average";

    /// <summary>
    /// Height in pixels for the heading section. 0 = auto-height based on content (default)
    /// </summary>
    public double HeadingHeightPixels { get; set; } = 0;

    /// <summary>
    /// Height in pixels for the disclaimer section. 0 = auto-height based on content (default)
    /// </summary>
    public double DisclaimerHeightPixels { get; set; } = 0;

    /// <summary>
    /// Minimal width in pixels for the table. 0 = auto-width based on content (default)
    /// Ensures the table doesn't shrink below this width
    /// </summary>
    public double TableMinimalWidthPixels { get; set; } = 0;

    /// <summary>
    /// Show borders around heading section (default: false)
    /// </summary>
    public bool ShowHeadingBorders { get; set; } = false;

    /// <summary>
    /// Show borders around disclaimer section (default: false)
    /// </summary>
    public bool ShowDisclaimerBorders { get; set; } = false;
}

/// <summary>
/// Interface for generic PDF export service
/// </summary>
public interface IPdfExportService
{
    /// <summary>
    /// Export a list of objects to PDF
    /// </summary>
    /// <typeparam name="T">Type of objects to export</typeparam>
    /// <param name="data">List of objects to export</param>
    /// <param name="columns">Column definitions</param>
    /// <param name="options">Export options (optional)</param>
    /// <returns>PDF file as byte array</returns>
    byte[] ExportToPdf<T>(
        IEnumerable<T> data,
        IList<PdfColumnDefinition> columns,
        PdfExportOptions? options = null);
}

/// <summary>
/// Generic PDF export service for exporting any list of objects to PDF
/// </summary>
public class PdfExportService : IPdfExportService
{
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

        // Set document title metadata (defaults to ReportTitle if DocumentTitle is not specified)
        document.Info.Title = !string.IsNullOrEmpty(options.DocumentTitle)
            ? options.DocumentTitle
            : options.ReportTitle;

        // Set document author metadata
        if (!string.IsNullOrEmpty(options.Author))
        {
            document.Info.Author = options.Author;
        }

        // Set document description/subject metadata
        if (!string.IsNullOrEmpty(options.Description))
        {
            document.Info.Subject = options.Description;
        }

        var page = document.AddPage();
        page.Size = options.PageSize;

        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont(options.FontFamily, options.FontSize);
        var fontBold = new XFont(options.FontFamily, options.HeaderFontSize, XFontStyle.Bold);
        var fontHeader = new XFont(options.FontFamily, options.TitleFontSize, XFontStyle.Bold);
        var fontFooter = new XFont(options.FontFamily, options.FooterFontSize);
        var fontPageNum = new XFont(options.FontFamily, options.PageNumberFontSize);

        // Calculate column widths
        var columnWidths = CalculateColumnWidths(gfx, dataList, columns, page, options, font, fontBold);
        var tableWidth = columnWidths.Sum();

        // Calculate centered table position
        var tableLeftX = (page.Width - tableWidth) / 2;

        var currentPageNum = 1;
        var itemsOnCurrentPage = 0;
        var firstItemOnPage = 1;

        // Calculate total pages by simulating pagination (accounts for both ItemsPerPage and height limits)
        int totalPages = CalculateTotalPages(dataList.Count, options);

        // Draw first page header and calculate where table should start
        double headerHeight = DrawPageHeader(page, gfx, options, font, fontHeader, tableWidth, tableLeftX);
        // Table starts immediately after header (no gap)
        var firstPageTableTop = headerHeight;
        double yPos = DrawTableHeader(gfx, firstPageTableTop, tableLeftX, columnWidths, columns, fontBold, options);

        for (int i = 0; i < dataList.Count; i++)
        {
            var item = dataList[i];

            // Draw data row
            DrawDataRow(gfx, item, yPos, tableLeftX, columnWidths, columns, font, options);
            yPos += options.RowHeight;
            itemsOnCurrentPage++;

            // Check if we need a new page
            if (itemsOnCurrentPage >= options.ItemsPerPage || yPos + options.RowHeight > page.Height - options.MarginBottom - 50)
            {
                // Draw footer for current page BEFORE creating new page
                DrawPageFooter(page, gfx, currentPageNum, totalPages, fontPageNum, options);

                // Create new page
                page = document.AddPage();
                page.Size = options.PageSize;
                gfx = XGraphics.FromPdfPage(page);
                currentPageNum++;
                firstItemOnPage = i + 1;
                itemsOnCurrentPage = 0;

                // Table starts closer to top on continuation pages
                yPos = 50;
                yPos = DrawTableHeader(gfx, yPos, tableLeftX, columnWidths, columns, fontBold, options);
            }

        }

        // Draw footer for first page if only one page
        if (currentPageNum == 1)
        {
            DrawPageFooter(page, gfx, currentPageNum, totalPages, fontPageNum, options);
        }

        // Draw average row (if enabled)
        if (options.ShowAverageRow)
        {
            yPos = DrawAverageRow(gfx, yPos, tableLeftX, dataList, columnWidths, columns, font, fontBold, options);
        }

        // Draw custom last row if provided
        if (options.LastRow != null && options.LastRow.CellValues.Count > 0)
        {
            yPos = DrawLastRow(gfx, yPos, tableLeftX, columnWidths, columns, font, fontBold, options);
        }

        // Draw disclaimer immediately after last table row (if provided)
        if (options.DisclaimerLines != null && options.DisclaimerLines.Count > 0)
        {
            yPos = DrawDisclaimer(gfx, yPos, page, options, fontFooter, fontBold, tableWidth, tableLeftX);
        }

        // Draw footer on last page
        DrawPageFooter(page, gfx, currentPageNum, totalPages, fontPageNum, options);

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

        // Enforce minimal table width if specified
        if (options.TableMinimalWidthPixels > 0)
        {
            double currentTotalWidth = widths.Sum();
            double minimalWidthPoints = options.TableMinimalWidthPixels * 0.75; // Convert pixels to points

            if (currentTotalWidth < minimalWidthPoints)
            {
                double widthDifference = minimalWidthPoints - currentTotalWidth;
                double widthToAddPerColumn = widthDifference / columns.Count;

                for (int i = 0; i < columns.Count; i++)
                {
                    widths[i] += widthToAddPerColumn;
                }
            }
        }

        return widths;
    }

    /// <summary>
    /// Get property value from object using reflection
    /// </summary>
    private object? GetPropertyValue<T>(T item, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
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
    /// Draws text with support for **bold** markup. Use **text** for bold portions.
    /// The bold text will have the same font size as the regular text.
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

        // Create bold font with same size as regular font for consistent sizing
        var fontBoldSameSize = new XFont(font.FontFamily.Name, font.Size, XFontStyle.Bold);

        // Calculate total width for alignment
        double totalWidth = 0;
        foreach (var (segText, isBold) in segments)
        {
            var segFont = isBold ? fontBoldSameSize : font;
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

            var segFont = isBold ? fontBoldSameSize : font;
            gfx.DrawString(segText, segFont, brush,
                new XRect(currentX, yPos, maxWidth, lineHeight), XStringFormats.TopLeft);
            currentX += gfx.MeasureString(segText, segFont).Width;
        }
    }

    /// <summary>
    /// Draw first page header (report title and header lines)
    /// Returns the Y position after the header (header bottom edge)
    /// </summary>
    private double DrawPageHeader(PdfPage page, XGraphics gfx, PdfExportOptions options, XFont font, XFont fontHeader, double tableWidth, double tableLeftX)
    {
        double yPos = 30;
        const double TitlePaddingTop = 10;
        const double HeaderPadding = 8;
        double headerStartY = yPos; // Store start position for border

        // Draw title with top padding
        yPos += TitlePaddingTop;
        double titleLineHeight = options.HeadingHeightPixels > 0 ? options.HeadingHeightPixels * 0.75 : 20;
        gfx.DrawString(options.ReportTitle, fontHeader, XBrushes.Black,
            new XRect(tableLeftX + HeaderPadding, yPos, tableWidth - HeaderPadding, titleLineHeight), XStringFormats.TopLeft);
        yPos += titleLineHeight + 10;

        // Draw header lines (multi-line with **bold** markup support)
        double headerLineHeight = 15;
        if (options.HeaderLines != null && options.HeaderLines.Count > 0)
        {
            foreach (var line in options.HeaderLines)
            {
                // Split by \r\n for multi-line display within a single header line
                var subLines = line.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (var subLine in subLines)
                {
                    DrawFormattedText(gfx, subLine, tableLeftX + HeaderPadding, yPos, tableWidth - HeaderPadding, headerLineHeight, font, fontHeader, XBrushes.Black, XStringAlignment.Near);
                    yPos += headerLineHeight;
                }
            }
        }

        // Draw border around entire header section (title + header lines) if enabled
        if (options.ShowHeadingBorders)
        {
            var pen = new XPen(XColors.Gray, 0.5);
            double headerHeight = yPos - headerStartY + 5;
            gfx.DrawRectangle(pen, tableLeftX, headerStartY, tableWidth, headerHeight);
            yPos += 5;
        }

        return yPos;
    }

    /// <summary>
    /// Draw table header row with Excel-like grid style
    /// Returns the Y position after the header row
    /// </summary>
    private double DrawTableHeader(
        XGraphics gfx,
        double yPos,
        double tableLeftX,
        List<double> columnWidths,
        IList<PdfColumnDefinition> columns,
        XFont fontBold,
        PdfExportOptions options)
    {
        var pen = new XPen(XColors.Black, 0.5); // Black border for Excel-like style
        var brushHeader = new XSolidBrush(XColor.FromArgb(217, 217, 217)); // Light gray header

        // Calculate max number of lines across all headers (for multi-line support with \r\n)
        int maxLines = 1;
        foreach (var col in columns)
        {
            var lines = col.HeaderText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            maxLines = Math.Max(maxLines, lines.Length);
        }

        // Calculate header row height based on number of lines
        double headerRowHeight = options.RowHeight * maxLines;
        double lineHeight = options.RowHeight;

        double xPos = tableLeftX;

        for (int i = 0; i < columns.Count; i++)
        {
            var width = columnWidths[i];
            var headerText = columns[i].HeaderText;

            // Draw the header cell background
            gfx.DrawRectangle(pen, brushHeader, xPos, yPos, width, headerRowHeight);

            // Split by \r\n for multi-line display
            var lines = headerText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            // Calculate starting Y to center the text block vertically
            double textBlockHeight = lines.Length * lineHeight;
            double currentY = yPos + (headerRowHeight - textBlockHeight) / 2;

            // Draw each line of the header text
            // Use HeaderAlignment if set, otherwise fall back to Alignment, default to Center
            var headerAlignment = columns[i].HeaderAlignment ?? columns[i].Alignment;
            var stringFormat = headerAlignment switch
            {
                XStringAlignment.Near => XStringFormats.CenterLeft,
                XStringAlignment.Far => XStringFormats.CenterRight,
                _ => XStringFormats.Center
            };

            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    gfx.DrawString(line, fontBold, XBrushes.Black,
                        new XRect(xPos, currentY, width, lineHeight), stringFormat);
                }
                currentY += lineHeight;
            }

            xPos += width;
        }

        return yPos + headerRowHeight;
    }

    /// <summary>
    /// Draw a data row with Excel-like grid style
    /// </summary>
    private void DrawDataRow<T>(
        XGraphics gfx,
        T item,
        double yPos,
        double tableLeftX,
        List<double> columnWidths,
        IList<PdfColumnDefinition> columns,
        XFont font,
        PdfExportOptions options)
    {
        var pen = new XPen(XColors.Black, 0.5); // Black border for Excel-like style
        var brushWhite = new XSolidBrush(XColors.White);

        double xPos = tableLeftX;

        for (int i = 0; i < columns.Count; i++)
        {
            var width = columnWidths[i];
            var value = GetPropertyValue(item, columns[i].PropertyName);
            var text = FormatValue(value, columns[i]);

            gfx.DrawRectangle(pen, brushWhite, xPos, yPos, width, options.RowHeight);

            // Use HeaderAlignment if set, otherwise fall back to Alignment (follows header alignment)
            var format = columns[i].HeaderAlignment ?? columns[i].Alignment;
            if (format == XStringAlignment.Center)
            {
                gfx.DrawString(text, font, XBrushes.Black,
                    new XRect(xPos, yPos, width, options.RowHeight), XStringFormats.Center);
            }
            else if (format == XStringAlignment.Near)
            {
                // For left-aligned text, add 5 units left padding
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
    /// Draw average row with grey background and Excel-like grid
    /// </summary>
    private double DrawAverageRow<T>(
        XGraphics gfx,
        double yPos,
        double tableLeftX,
        List<T> dataList,
        List<double> columnWidths,
        IList<PdfColumnDefinition> columns,
        XFont font,
        XFont fontBold,
        PdfExportOptions options)
    {
        var pen = new XPen(XColors.Black, 0.5); // Black border for Excel-like style
        var brushGray = new XSolidBrush(XColor.FromArgb(240, 240, 240));

        double xPos = tableLeftX;

        for (int i = 0; i < columns.Count; i++)
        {
            var width = columnWidths[i];
            var column = columns[i];

            // Draw grey background with border for the cell
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
    /// Draw custom last row with user-provided values
    /// </summary>
    private double DrawLastRow(
        XGraphics gfx,
        double yPos,
        double tableLeftX,
        List<double> columnWidths,
        IList<PdfColumnDefinition> columns,
        XFont font,
        XFont fontBold,
        PdfExportOptions options)
    {
        var pen = new XPen(XColors.Black, 0.5); // Black border for Excel-like style
        var brushGray = new XSolidBrush(XColor.FromArgb(240, 240, 240));

        // Determine font style for last row
        XFont rowFont;
        if (options.LastRow!.FontStyle != null)
        {
            var fontStyle = XFontStyle.Regular;
            if (options.LastRow.FontStyle.Bold) fontStyle |= XFontStyle.Bold;
            if (options.LastRow.FontStyle.Italic) fontStyle |= XFontStyle.Italic;
            rowFont = new XFont(
                options.LastRow.FontStyle.FontFamily,
                options.LastRow.FontStyle.FontSize,
                fontStyle
            );
        }
        else
        {
            rowFont = fontBold; // Default to bold font for last row
        }

        const double LastRowPadding = 8;
        double xPos = tableLeftX;

        for (int i = 0; i < columns.Count; i++)
        {
            var width = columnWidths[i];

            // Draw grey background with border for cell
            gfx.DrawRectangle(pen, brushGray, xPos, yPos, width, options.RowHeight);

            // Use provided value if available, otherwise empty string
            string cellValue = i < options.LastRow!.CellValues.Count
                ? options.LastRow.CellValues[i]
                : "";

            // Draw cell value with alignment and padding
            if(i == 0){
                // First column: left-aligned with padding
                gfx.DrawString(cellValue, rowFont, XBrushes.Black,
                    new XRect(xPos + LastRowPadding, yPos, width - LastRowPadding, options.RowHeight), XStringFormats.CenterLeft);
            }else{
                // Other columns: right-aligned with padding
                gfx.DrawString(cellValue, rowFont, XBrushes.Black,
                    new XRect(xPos, yPos, width - LastRowPadding, options.RowHeight), XStringFormats.CenterRight);
            }
            xPos += width;
        }

        return yPos + options.RowHeight;
    }
    /// Check if a value is numeric
    /// </summary>
    private bool IsNumeric(object value)
    {
        return value is int or double or decimal or float or long or short or byte or uint or ulong or ushort or sbyte;
    }

    /// <summary>
    /// Draw disclaimer immediately after table rows
    /// </summary>
    private double DrawDisclaimer(
        XGraphics gfx,
        double yPos,
        PdfPage page,
        PdfExportOptions options,
        XFont fontFooter,
        XFont fontBold,
        double tableWidth,
        double tableLeftX)
    {
        const double DisclaimerPadding = 8;

        // Disclaimer starts immediately after table (no gap)
        // Store start position for border drawing
        double disclaimerStartY = yPos;

        // Draw disclaimer text (multi-line with **bold** markup support)
        double disclaimerLineHeight = 12;
        if (options.DisclaimerLines != null && options.DisclaimerLines.Count > 0)
        {
            foreach (var line in options.DisclaimerLines)
            {
                // Split by \r\n for multi-line display within a single disclaimer line
                var subLines = line.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (var subLine in subLines)
                {
                    DrawFormattedText(gfx, subLine, tableLeftX + DisclaimerPadding, yPos, tableWidth - DisclaimerPadding, disclaimerLineHeight, fontFooter, fontBold, XBrushes.Black, XStringAlignment.Near);
                    yPos += disclaimerLineHeight;
                }
            }
        }

        // Draw border around disclaimer section if enabled
        if (options.ShowDisclaimerBorders)
        {
            var pen = new XPen(XColors.Black, 0.5);
            double disclaimerHeight = yPos - disclaimerStartY + 5;
            gfx.DrawRectangle(pen, tableLeftX, disclaimerStartY, tableWidth, disclaimerHeight);
            yPos += 5;
        }

        return yPos;
    }

    /// <summary>
    /// Get page height based on PageSize enum
    /// </summary>
    private double GetPageHeight(PageSize pageSize)
    {
        return pageSize switch
        {
            PageSize.A4 => 842,
            PageSize.Letter => 792,
            _ => 842 // Default to A4
        };
    }

    /// <summary>
    /// Calculate total pages by simulating pagination with both ItemsPerPage and height limits
    /// </summary>
    private int CalculateTotalPages(int itemCount, PdfExportOptions options)
    {
        if (itemCount == 0) return 1;

        double pageHeight = GetPageHeight(options.PageSize);
        double yPos = options.MarginTop + 100; // Estimated starting position (header height)
        int pages = 1;
        int itemsOnCurrentPage = 0;

        for (int i = 0; i < itemCount; i++)
        {
            // Check if adding next row exceeds page limits
            if (itemsOnCurrentPage >= options.ItemsPerPage || 
                yPos + options.RowHeight > pageHeight - options.MarginBottom - 50)
            {
                pages++;
                itemsOnCurrentPage = 0;
                yPos = 50; // Reset to top for continuation pages
            }
            
            yPos += options.RowHeight;
            itemsOnCurrentPage++;
        }

        return pages;
    }

    /// <summary>
    /// Draw page footer: "page X of Total Pages"
    /// </summary>
    private void DrawPageFooter(
        PdfPage page,
        XGraphics gfx,
        int pageNum,
        int totalPages,
        XFont fontPageNum,
        PdfExportOptions options)
    {
        double footerY = page.Height - options.MarginBottom - 10;

        // Draw separator line above footer
        var separatorPen = new XPen(XColors.Gray, 0.5);
        gfx.DrawLine(separatorPen, 0, footerY - 5, page.Width, footerY - 5);

        // Only show "page X of Y" centered
        var pageText = $"page {pageNum} of {totalPages}";
        gfx.DrawString(pageText, fontPageNum, XBrushes.DarkGray,
            new XRect(0, footerY, page.Width, 15), XStringFormats.TopCenter);
    }
}
