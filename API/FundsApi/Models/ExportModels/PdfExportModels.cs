using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace FundsApi.Models.ExportModels;

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
    /// Calculate and show average for this column in summary row (only for numeric types)
    /// </summary>
    public bool ShowAverage { get; set; }
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
    /// Disclaimer text to display below the footer (optional). Multi-line disclaimers can use \n for line breaks.
    /// </summary>
    public string? Disclaimer { get; set; }

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
}
