namespace FundsApi.Models.ExportModels;

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
