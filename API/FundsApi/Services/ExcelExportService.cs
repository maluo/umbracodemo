using ClosedXML.Excel;
using FundsApi.Models.ExportModels;
using System.Text;

namespace FundsApi.Services;

/// <summary>
/// Generic Excel export service for exporting any list of objects to Excel
/// </summary>
public class ExcelExportService : IExcelExportService
{
    private readonly ILogger<ExcelExportService> _logger;

    public ExcelExportService(ILogger<ExcelExportService> logger)
    {
        _logger = logger;
    }

    public byte[] ExportToExcel<T>(
        IEnumerable<T> data,
        IList<ExcelColumnDefinition> columns,
        ExcelExportOptions? options = null)
    {
        options ??= new ExcelExportOptions();
        var dataList = data.ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(options.WorksheetName);

        int currentRow = 1;

        // Draw title
        currentRow = DrawTitle(worksheet, currentRow, columns.Count, options);

        // Draw table header
        DrawTableHeader(worksheet, currentRow, columns, options);
        currentRow++;

        // Draw data rows
        DrawDataRows(worksheet, currentRow, dataList, columns, options);
        currentRow += dataList.Count;

        // Draw custom last row if provided
        if (options.LastRow != null && options.LastRow.CellValues.Count > 0)
        {
            DrawLastRow(worksheet, currentRow, columns, options);
            currentRow++;
        }

        // Draw footer
        currentRow = DrawFooter(worksheet, currentRow, columns.Count, options);

        // Draw disclaimer
        currentRow = DrawDisclaimer(worksheet, currentRow, columns.Count, options);

        // Auto-fit columns if enabled
        if (options.AutoFitColumns)
        {
            worksheet.Columns().AdjustToContents();
        }

        // Enforce minimal table width if specified
        if (options.TableMinimalWidthPixels > 0)
        {
            EnforceMinimalTableWidth(worksheet, columns.Count, options.TableMinimalWidthPixels);
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// Enforce minimal table width by adjusting column widths if needed
    /// </summary>
    private void EnforceMinimalTableWidth(IXLWorksheet worksheet, int columnCount, double minimalWidthPixels)
    {
        // Calculate current total width in pixels
        // Excel column width is roughly in characters, 1 character ≈ 7 pixels at default zoom/font
        const double pixelsPerCharacter = 7.0;
        double currentTotalWidth = 0;

        for (int i = 1; i <= columnCount; i++)
        {
            currentTotalWidth += worksheet.Column(i).Width * pixelsPerCharacter;
        }

        // If current width is less than minimal, distribute the difference
        if (currentTotalWidth < minimalWidthPixels)
        {
            double widthDifference = minimalWidthPixels - currentTotalWidth;
            double widthToAddPerColumn = widthDifference / columnCount;

            for (int i = 1; i <= columnCount; i++)
            {
                double currentWidth = worksheet.Column(i).Width;
                double currentWidthInPixels = currentWidth * pixelsPerCharacter;
                worksheet.Column(i).Width = (currentWidthInPixels + widthToAddPerColumn) / pixelsPerCharacter;
            }
        }
    }

    /// <summary>
    /// Draw title and subtitle
    /// </summary>
    private int DrawTitle(IXLWorksheet worksheet, int startRow, int columnCount, ExcelExportOptions options)
    {
        int currentRow = startRow;

        // Draw title in a single merged cell with text wrapping
        var titleCell = worksheet.Cell(currentRow, 1);
        titleCell.Value = options.ReportTitle;
        ApplyRichText(titleCell, options.ReportTitle, options.TitleFont, allowBoldFromMarkers: true);
        var titleRange = worksheet.Range(currentRow, 1, currentRow, columnCount);
        titleRange.Merge();
        titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        titleRange.Style.Alignment.WrapText = true;

        // Apply or remove borders based on ShowHeadingBorders option
        if (options.ShowHeadingBorders)
        {
            titleRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            titleRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            titleRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            titleRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            titleRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
        }
        else
        {
            // Remove all borders from merged range
            titleRange.Style.Border.TopBorder = XLBorderStyleValues.None;
            titleRange.Style.Border.BottomBorder = XLBorderStyleValues.None;
            titleRange.Style.Border.LeftBorder = XLBorderStyleValues.None;
            titleRange.Style.Border.RightBorder = XLBorderStyleValues.None;
            titleRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
        }

        // Apply custom heading height if specified (convert pixels to points: 1 pixel = 0.75 points)
        if (options.HeadingHeightPixels > 0)
        {
            worksheet.Row(currentRow).Height = options.HeadingHeightPixels * 0.75;
        }

        currentRow++;

        // Draw subtitle in a single merged cell with text wrapping if provided
        if (!string.IsNullOrEmpty(options.Subtitle))
        {
            var subtitleCell = worksheet.Cell(currentRow, 1);
            subtitleCell.Value = options.Subtitle;
            ApplyRichText(subtitleCell, options.Subtitle, options.SubtitleFont, allowBoldFromMarkers: true);
            var subtitleRange = worksheet.Range(currentRow, 1, currentRow, columnCount);
            subtitleRange.Merge();
            subtitleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            subtitleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            subtitleRange.Style.Alignment.WrapText = true;

            // Apply or remove borders based on ShowHeadingBorders option
            if (options.ShowHeadingBorders)
            {
                subtitleRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                subtitleRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                subtitleRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                subtitleRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                subtitleRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
            }
            else
            {
                // Remove all borders from merged range
                subtitleRange.Style.Border.TopBorder = XLBorderStyleValues.None;
                subtitleRange.Style.Border.BottomBorder = XLBorderStyleValues.None;
                subtitleRange.Style.Border.LeftBorder = XLBorderStyleValues.None;
                subtitleRange.Style.Border.RightBorder = XLBorderStyleValues.None;
                subtitleRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
            }

            // Apply custom heading height if specified (convert pixels to points: 1 pixel = 0.75 points)
            if (options.HeadingHeightPixels > 0)
            {
                worksheet.Row(currentRow).Height = options.HeadingHeightPixels * 0.75;
            }

            currentRow++;
        }

        return currentRow;
    }

    /// <summary>
    /// Draw table header row
    /// </summary>
    private void DrawTableHeader(IXLWorksheet worksheet, int row, IList<ExcelColumnDefinition> columns, ExcelExportOptions options)
    {
        for (int i = 0; i < columns.Count; i++)
        {
            var cell = worksheet.Cell(row, i + 1);
            cell.Value = columns[i].HeaderText;
            ApplyRichText(cell, columns[i].HeaderText, options.HeaderFont);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            // Set column width if specified
            if (columns[i].Width > 0)
            {
                worksheet.Column(i + 1).Width = columns[i].Width;
            }
        }
    }

    /// <summary>
    /// Draw data rows
    /// </summary>
    private void DrawDataRows<T>(IXLWorksheet worksheet, int startRow, List<T> data, IList<ExcelColumnDefinition> columns, ExcelExportOptions options)
    {
        for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
        {
            var item = data[rowIndex];
            int row = startRow + rowIndex;

            for (int colIndex = 0; colIndex < columns.Count; colIndex++)
            {
                var column = columns[colIndex];
                var cell = worksheet.Cell(row, colIndex + 1);

                var value = GetPropertyValue(item, column.PropertyName);
                var text = FormatValue(value, column);

                cell.Value = text;
                ApplyFontStyle(cell, options.DataFont);
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                if (options.ShowBorders)
                {
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }
            }
        }
    }

    /// <summary>
    /// Draw custom last row with user-provided values
    /// </summary>
    private void DrawLastRow(IXLWorksheet worksheet, int row, IList<ExcelColumnDefinition> columns, ExcelExportOptions options)
    {
        var fontStyle = options.LastRow?.FontStyle ?? options.DataFont;

        for (int colIndex = 0; colIndex < columns.Count; colIndex++)
        {
            var cell = worksheet.Cell(row, colIndex + 1);

            // Use provided value if available, otherwise empty string
            string cellValue = colIndex < options.LastRow!.CellValues.Count
                ? options.LastRow.CellValues[colIndex]
                : "";

            cell.Value = cellValue;
            ApplyFontStyle(cell, fontStyle);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            if (options.ShowBorders)
            {
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }
        }
    }

    /// <summary>
    /// Draw footer section
    /// </summary>
    private int DrawFooter(IXLWorksheet worksheet, int startRow, int columnCount, ExcelExportOptions options)
    {
        if (string.IsNullOrEmpty(options.FooterText))
            return startRow;

        int currentRow = startRow;

        // Draw footer in a single merged cell with text wrapping
        var footerCell = worksheet.Cell(currentRow, 1);
        footerCell.Value = options.FooterText;
        ApplyRichText(footerCell, options.FooterText, options.FooterFont);
        var footerRange = worksheet.Range(currentRow, 1, currentRow, columnCount);
        footerRange.Merge();
        footerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        footerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        footerRange.Style.Alignment.WrapText = true;

        // Apply or remove borders based on ShowFooterBorders option
        if (options.ShowFooterBorders)
        {
            footerRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            footerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            footerRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            footerRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            footerRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
        }
        else
        {
            // Remove all borders from merged range
            footerRange.Style.Border.TopBorder = XLBorderStyleValues.None;
            footerRange.Style.Border.BottomBorder = XLBorderStyleValues.None;
            footerRange.Style.Border.LeftBorder = XLBorderStyleValues.None;
            footerRange.Style.Border.RightBorder = XLBorderStyleValues.None;
            footerRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
        }

        currentRow++;

        return currentRow;
    }

    /// <summary>
    /// Draw disclaimer section with multiline support and per-line rich text formatting
    /// </summary>
    private int DrawDisclaimer(IXLWorksheet worksheet, int startRow, int columnCount, ExcelExportOptions options)
    {
        if (string.IsNullOrEmpty(options.Disclaimer))
            return startRow;

        int currentRow = startRow;

        // Split disclaimer by newlines to support per-line formatting
        string[] disclaimerLines = options.Disclaimer.Split(new[] { "\\n" }, StringSplitOptions.None);

        // Draw each line as a separate row to enable per-line rich text formatting
        for (int lineIndex = 0; lineIndex < disclaimerLines.Length; lineIndex++)
        {
            var line = disclaimerLines[lineIndex];
            var disclaimerCell = worksheet.Cell(currentRow, 1);
            disclaimerCell.Value = line;
            ApplyRichText(disclaimerCell, line, options.DisclaimerFont, allowBoldFromMarkers: true);
            var disclaimerRange = worksheet.Range(currentRow, 1, currentRow, columnCount);
            disclaimerRange.Merge();
            disclaimerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            disclaimerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            disclaimerRange.Style.Alignment.WrapText = true;

            // Apply or remove borders based on ShowDisclaimerBorders option
            if (options.ShowDisclaimerBorders)
            {
                disclaimerRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                disclaimerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                disclaimerRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                disclaimerRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                disclaimerRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
            }
            else
            {
                // Remove all borders from merged range
                disclaimerRange.Style.Border.TopBorder = XLBorderStyleValues.None;
                disclaimerRange.Style.Border.BottomBorder = XLBorderStyleValues.None;
                disclaimerRange.Style.Border.LeftBorder = XLBorderStyleValues.None;
                disclaimerRange.Style.Border.RightBorder = XLBorderStyleValues.None;
                disclaimerRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
            }

            // Apply custom disclaimer height if specified (convert pixels to points: 1 pixel = 0.75 points)
            // If a custom height is set, divide it by the number of lines to distribute evenly
            if (options.DisclaimerHeightPixels > 0)
            {
                double heightPerLine = options.DisclaimerHeightPixels * 0.75 / disclaimerLines.Length;
                worksheet.Row(currentRow).Height = heightPerLine;
            }

            currentRow++;
        }

        return currentRow;
    }

    /// <summary>
    /// Apply rich text formatting with **bold** support. Use **text** for bold portions.
    /// Note: ClosedXML doesn't support partial cell formatting, so if ** is present, entire cell is bolded
    /// For title and subtitle: ** markers will bold the entire cell
    /// For footer: ** markers are stripped without applying bold (respects font style)
    /// For disclaimer: ** markers will bold the entire line (each line is drawn separately)
    /// </summary>
    /// <param name="cell">The cell to apply formatting to</param>
    /// <param name="text">The text with optional **bold** markers</param>
    /// <param name="fontStyle">The font style to apply</param>
    /// <param name="allowBoldFromMarkers">If true, ** markers will bold the entire cell. If false, markers are stripped only.</param>
    private void ApplyRichText(IXLCell cell, string text, ExcelFontStyle fontStyle, bool allowBoldFromMarkers = false)
    {
        // Strip ** markers and set value
        string cleanText = text.Replace("**", "");
        cell.Value = cleanText;

        // Only apply bold from ** markers if explicitly allowed (for title/subtitle)
        // For footer/disclaimer, we respect the font style setting and ignore ** markers for bold
        bool hasBold = text.Contains("**") && allowBoldFromMarkers;

        var style = new ExcelFontStyle
        {
            FontName = fontStyle.FontName,
            FontSize = fontStyle.FontSize,
            Bold = fontStyle.Bold || hasBold, // Bold if style has bold OR text has ** markers (and allowed)
            Italic = fontStyle.Italic,
            FontColor = fontStyle.FontColor,
            BackgroundColor = fontStyle.BackgroundColor
        };

        ApplyFontStyle(cell, style);
    }

    /// <summary>
    /// Apply font style to cell
    /// </summary>
    private void ApplyFontStyle(IXLCell cell, ExcelFontStyle fontStyle)
    {
        var font = cell.Style.Font;
        font.FontName = fontStyle.FontName;
        font.FontSize = fontStyle.FontSize;
        font.Bold = fontStyle.Bold;
        font.Italic = fontStyle.Italic;

        if (!string.IsNullOrEmpty(fontStyle.FontColor))
        {
            font.FontColor = XLColor.FromHtml(fontStyle.FontColor);
        }

        if (!string.IsNullOrEmpty(fontStyle.BackgroundColor))
        {
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(fontStyle.BackgroundColor);
        }
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
    private string FormatValue(object? value, ExcelColumnDefinition column)
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
    /// Convert ExcelAlignment to XLAlignmentHorizontalValues
    /// </summary>
    private XLAlignmentHorizontalValues GetAlignment(ExcelAlignment alignment)
    {
        return alignment switch
        {
            ExcelAlignment.Left => XLAlignmentHorizontalValues.Left,
            ExcelAlignment.Center => XLAlignmentHorizontalValues.Center,
            ExcelAlignment.Right => XLAlignmentHorizontalValues.Right,
            _ => XLAlignmentHorizontalValues.Center
        };
    }
}
