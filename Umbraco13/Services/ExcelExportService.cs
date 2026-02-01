using ClosedXML.Excel;
using System.Text;

namespace Umbraco13.Services;

/// <summary>
/// Defines a column for Excel export
/// </summary>
public class ExcelColumnDefinition
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
    /// Column width (0 = auto-fit)
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Optional format string for formatting values (e.g., "C2" for currency, "F2" for decimal)
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Optional custom formatter function
    /// </summary>
    public Func<object?, string>? CustomFormatter { get; set; }

    /// <summary>
    /// Cell alignment (default: Center)
    /// </summary>
    public ExcelAlignment Alignment { get; set; } = ExcelAlignment.Center;
}

/// <summary>
/// Cell alignment options
/// </summary>
public enum ExcelAlignment
{
    Left,
    Center,
    Right
}

/// <summary>
/// Font styling options
/// </summary>
public class ExcelFontStyle
{
    /// <summary>
    /// Font family name (default: "Calibri")
    /// </summary>
    public string FontName { get; set; } = "Calibri";

    /// <summary>
    /// Font size (default: 11)
    /// </summary>
    public double FontSize { get; set; } = 11;

    /// <summary>
    /// Bold text (default: false)
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Italic text (default: false)
    /// </summary>
    public bool Italic { get; set; }

    /// <summary>
    /// Font color (hex format, e.g., "#000000" for black)
    /// </summary>
    public string? FontColor { get; set; }

    /// <summary>
    /// Background color (hex format, e.g., "#FFFFFF" for white)
    /// </summary>
    public string? BackgroundColor { get; set; }
}

/// <summary>
/// Options for Excel export
/// </summary>
public class ExcelExportOptions
{
    /// <summary>
    /// Worksheet name (default: "Sheet1")
    /// </summary>
    public string WorksheetName { get; set; } = "Sheet1";

    /// <summary>
    /// Report title (multi-line supported with \n, use **bold** for bold text)
    /// </summary>
    public string ReportTitle { get; set; } = "Report";

    /// <summary>
    /// Subtitle (multi-line supported with \n, use **bold** for bold text)
    /// </summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// Footer text (multi-line supported with \n, use **bold** for bold text)
    /// </summary>
    public string? FooterText { get; set; }

    /// <summary>
    /// Disclaimer text (multi-line supported with \n, use **bold** for bold text)
    /// </summary>
    public string? Disclaimer { get; set; }

    // Header styling
    /// <summary>
    /// Title font style
    /// </summary>
    public ExcelFontStyle TitleFont { get; set; } = new ExcelFontStyle { FontSize = 16, Bold = true };

    /// <summary>
    /// Subtitle font style
    /// </summary>
    public ExcelFontStyle SubtitleFont { get; set; } = new ExcelFontStyle { FontSize = 11 };

    /// <summary>
    /// Header row font style
    /// </summary>
    public ExcelFontStyle HeaderFont { get; set; } = new ExcelFontStyle { Bold = true, BackgroundColor = "#E0E0E0" };

    /// <summary>
    /// Data row font style
    /// </summary>
    public ExcelFontStyle DataFont { get; set; } = new ExcelFontStyle { FontSize = 10 };

    /// <summary>
    /// Footer font style
    /// </summary>
    public ExcelFontStyle FooterFont { get; set; } = new ExcelFontStyle { FontSize = 9, Italic = true };

    /// <summary>
    /// Disclaimer font style
    /// </summary>
    public ExcelFontStyle DisclaimerFont { get; set; } = new ExcelFontStyle { FontSize = 8, FontColor = "#000000" };

    // Header options
    /// <summary>
    /// Rows to skip after title (default: 1)
    /// </summary>
    public int RowsAfterTitle { get; set; } = 1;

    /// <summary>
    /// Rows to skip after subtitle (default: 1)
    /// </summary>
    public int RowsAfterSubtitle { get; set; } = 1;

    // Data options
    /// <summary>
    /// Auto-fit column widths (default: true)
    /// </summary>
    public bool AutoFitColumns { get; set; } = true;

    /// <summary>
    /// Show table borders (default: true)
    /// </summary>
    public bool ShowBorders { get; set; } = true;
}

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

        // Draw footer
        currentRow = DrawFooter(worksheet, currentRow, columns.Count, options);

        // Draw disclaimer
        currentRow = DrawDisclaimer(worksheet, currentRow, columns.Count, options);

        // Auto-fit columns if enabled
        if (options.AutoFitColumns)
        {
            worksheet.Columns().AdjustToContents();
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
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
        ApplyRichText(titleCell, options.ReportTitle, options.TitleFont);
        var titleRange = worksheet.Range(currentRow, 1, currentRow, columnCount);
        titleRange.Merge();
        titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        titleRange.Style.Alignment.WrapText = true;
        // Remove all borders from merged range
        titleRange.Style.Border.TopBorder = XLBorderStyleValues.None;
        titleRange.Style.Border.BottomBorder = XLBorderStyleValues.None;
        titleRange.Style.Border.LeftBorder = XLBorderStyleValues.None;
        titleRange.Style.Border.RightBorder = XLBorderStyleValues.None;
        titleRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
        currentRow++;

        // Draw subtitle in a single merged cell with text wrapping if provided
        if (!string.IsNullOrEmpty(options.Subtitle))
        {
            var subtitleCell = worksheet.Cell(currentRow, 1);
            subtitleCell.Value = options.Subtitle;
            ApplyRichText(subtitleCell, options.Subtitle, options.SubtitleFont);
            var subtitleRange = worksheet.Range(currentRow, 1, currentRow, columnCount);
            subtitleRange.Merge();
            subtitleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            subtitleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            subtitleRange.Style.Alignment.WrapText = true;
            // Remove all borders from merged range
            subtitleRange.Style.Border.TopBorder = XLBorderStyleValues.None;
            subtitleRange.Style.Border.BottomBorder = XLBorderStyleValues.None;
            subtitleRange.Style.Border.LeftBorder = XLBorderStyleValues.None;
            subtitleRange.Style.Border.RightBorder = XLBorderStyleValues.None;
            subtitleRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
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
        // Remove all borders from merged range
        footerRange.Style.Border.TopBorder = XLBorderStyleValues.None;
        footerRange.Style.Border.BottomBorder = XLBorderStyleValues.None;
        footerRange.Style.Border.LeftBorder = XLBorderStyleValues.None;
        footerRange.Style.Border.RightBorder = XLBorderStyleValues.None;
        footerRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
        currentRow++;

        return currentRow;
    }

    /// <summary>
    /// Draw disclaimer section
    /// </summary>
    private int DrawDisclaimer(IXLWorksheet worksheet, int startRow, int columnCount, ExcelExportOptions options)
    {
        if (string.IsNullOrEmpty(options.Disclaimer))
            return startRow;

        int currentRow = startRow;

        // Draw disclaimer in a single merged cell with text wrapping
        var disclaimerCell = worksheet.Cell(currentRow, 1);
        disclaimerCell.Value = options.Disclaimer;
        ApplyRichText(disclaimerCell, options.Disclaimer, options.DisclaimerFont);
        var disclaimerRange = worksheet.Range(currentRow, 1, currentRow, columnCount);
        disclaimerRange.Merge();
        disclaimerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        disclaimerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        disclaimerRange.Style.Alignment.WrapText = true;
        // Remove all borders from merged range
        disclaimerRange.Style.Border.TopBorder = XLBorderStyleValues.None;
        disclaimerRange.Style.Border.BottomBorder = XLBorderStyleValues.None;
        disclaimerRange.Style.Border.LeftBorder = XLBorderStyleValues.None;
        disclaimerRange.Style.Border.RightBorder = XLBorderStyleValues.None;
        disclaimerRange.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
        currentRow++;

        return currentRow;
    }

    /// <summary>
    /// Apply rich text formatting with **bold** support
    /// Note: ClosedXML doesn't support partial cell formatting, so if ** is present, entire cell is bolded
    /// </summary>
    private void ApplyRichText(IXLCell cell, string text, ExcelFontStyle fontStyle)
    {
        // Strip ** markers and set value
        string cleanText = text.Replace("**", "");
        cell.Value = cleanText;

        // Check if text had **bold** markers - if so, apply bold to entire cell
        bool hasBold = text.Contains("**");

        var style = new ExcelFontStyle
        {
            FontName = fontStyle.FontName,
            FontSize = fontStyle.FontSize,
            Bold = fontStyle.Bold || hasBold, // Bold if style has bold OR text has ** markers
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
