using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp;
using System.Reflection;
using System.Linq;

namespace PdfTester;

/// <summary>
/// PDF Generation Service - Converts generic list of objects to PDF using PdfSharp
/// </summary>
public class PdfService
{
    /// <summary>
    /// Options for PDF generation
    /// </summary>
    public class PdfOptions
    {
        /// <summary>Document title/header text</summary>
        public string Title { get; set; } = "Document";

        /// <summary>Header section content (supports multiple lines with bold markup)</summary>
        public List<string> HeaderLines { get; set; } = new();

        /// <summary>Disclaimer section content (supports multiple lines with bold markup)</summary>
        public List<string> DisclaimerLines { get; set; } = new();

        /// <summary>Whether to include page numbers</summary>
        public bool IncludePageNumbers { get; set; } = true;

        /// <summary>Font size for table content</summary>
        public int TableFontSize { get; set; } = 10;

        /// <summary>Whether to use Excel-like column widths (auto-fit)</summary>
        public bool UseExcelLikeSizing { get; set; } = true;
    }

    /// <summary>
    /// Generate a PDF document from a list of objects
    /// </summary>
    /// <typeparam name="T">Type of objects in the list</typeparam>
    /// <param name="data">List of objects to display in the table</param>
    /// <param name="outputPath">Path where the PDF will be saved</param>
    /// <param name="options">PDF generation options</param>
    public void GeneratePdf<T>(IEnumerable<T> data, string outputPath, PdfOptions? options = null)
    {
        options ??= new PdfOptions();

        var pdfBytes = GeneratePdf(data, options);
        File.WriteAllBytes(outputPath, pdfBytes);
    }

    /// <summary>
    /// Generate a PDF document and return as byte array
    /// </summary>
    /// <typeparam name="T">Type of objects in the list</typeparam>
    /// <param name="data">List of objects to display in the table</param>
    /// <param name="options">PDF generation options</param>
    /// <returns>PDF document as byte array</returns>
    public byte[] GeneratePdf<T>(IEnumerable<T> data, PdfOptions? options = null)
    {
        options ??= new PdfOptions();

        var document = new GenericPdfDocument<T>(data.ToList(), options);
        return document.Generate();
    }
}

/// <summary>
/// Generic PDF document implementation using PdfSharp
/// </summary>
internal class GenericPdfDocument<T>
{
    private readonly List<T> _data;
    private readonly PdfService.PdfOptions _options;
    private readonly List<PropertyInfo> _properties;
    private double[] _columnWidths = null!;
    private double _tableWidth = 0;

    // Page configuration
    private const double PageWidth = 595; // A4 width in points
    private const double PageHeight = 842; // A4 height in points
    private const double MarginLeft = 40;
    private const double MarginRight = 40;
    private const double MarginTop = 40;
    private const double MarginBottom = 40;
    private const double ContentWidth = PageWidth - MarginLeft - MarginRight;

    public GenericPdfDocument(List<T> data, PdfService.PdfOptions options)
    {
        _data = data;
        _options = options;
        _properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .ToList();
    }

    public byte[] Generate()
    {
        var document = new PdfDocument();
        document.Info.Title = _options.Title;

        // Calculate column widths based on content before rendering
        _columnWidths = CalculateColumnWidths();
        _tableWidth = _columnWidths.Sum();

        // Track if header has been added (only on first page)
        bool headerAdded = false;
        // Track if disclaimer has been added (only after table on last page)
        bool disclaimerAdded = false;

        int rowsPerPage = CalculateRowsPerPage();
        int totalPages = (int)Math.Ceiling((double)_data.Count / rowsPerPage);

        for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
        {
            var page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            page.Width = PageWidth;
            page.Height = PageHeight;

            var graphics = XGraphics.FromPdfPage(page);
            double yPos = MarginTop;

            // Add header only on first page
            if (!headerAdded)
            {
                yPos = AddHeader(graphics, yPos);
                headerAdded = true;
            }

            // Add table
            int startIndex = pageIndex * rowsPerPage;
            int endIndex = Math.Min(startIndex + rowsPerPage, _data.Count);
            yPos = AddTable(graphics, yPos, startIndex, endIndex, pageIndex == 0);

            // Add disclaimer after table on last page only
            if (pageIndex == totalPages - 1 && !disclaimerAdded)
            {
                yPos = AddDisclaimer(graphics, yPos);
                disclaimerAdded = true;
            }

            // Add page number
            if (_options.IncludePageNumbers)
            {
                AddPageNumber(graphics, pageIndex + 1, totalPages);
            }
        }

        // Return as byte array
        using (var stream = new MemoryStream())
        {
            document.Save(stream);
            return stream.ToArray();
        }
    }

    private int CalculateRowsPerPage()
    {
        // Reserve space for header (100), disclaimer (50), and footer (20)
        var availableHeight = PageHeight - MarginTop - MarginBottom - 170;
        var rowHeight = _options.TableFontSize + 10; // Row height with padding
        return (int)(availableHeight / rowHeight);
    }

    private double AddHeader(XGraphics graphics, double yPos)
    {
        var titleFont = new XFont("Arial", 18, XFontStyle.Bold);
        var textFont = new XFont("Arial", 10, XFontStyle.Regular);
        var boldFont = new XFont("Arial", 10, XFontStyle.Bold);

        double startY = yPos;

        // Title
        graphics.DrawString(_options.Title, titleFont, XBrushes.DarkBlue,
            new XRect(MarginLeft, yPos, _tableWidth, 30),
            XStringFormats.TopLeft);
        yPos += 30;

        // Header lines
        foreach (var line in _options.HeaderLines)
        {
            DrawFormattedText(graphics, line, MarginLeft, yPos, textFont, boldFont, _tableWidth);
            yPos += 18;
        }

        // Draw grid box around header content (matches table width exactly)
        double headerHeight = yPos - startY;
        var gridRect = new XRect(MarginLeft, startY, _tableWidth, headerHeight);
        graphics.DrawRectangle(new XPen(XColors.Gray, 1), gridRect);

        return yPos;
    }

    private double AddTable(XGraphics graphics, double yPos, int startIndex, int endIndex, bool isFirstPage)
    {
        var headerFont = new XFont("Arial", _options.TableFontSize, XFontStyle.Bold);
        var cellFont = new XFont("Arial", _options.TableFontSize, XFontStyle.Regular);
        const double CellPadding = 4;

        // Use pre-calculated column widths
        var colWidths = _columnWidths;

        // Draw table header
        if (isFirstPage || startYPos > yPos)
        {
            double xPos = MarginLeft;
            var headerRect = new XRect(xPos, yPos, colWidths[0], 20);
            graphics.DrawRectangle(XBrushes.LightGray, headerRect);
            DrawCellBorder(graphics, headerRect);
            graphics.DrawString("Row #", headerFont, XBrushes.Black,
                new XRect(xPos + CellPadding, yPos, colWidths[0] - CellPadding, 20), XStringFormats.CenterLeft);
            xPos += colWidths[0];

            for (int i = 0; i < _properties.Count; i++)
            {
                var cellRect = new XRect(xPos, yPos, colWidths[i + 1], 20);
                graphics.DrawRectangle(XBrushes.LightGray, cellRect);
                DrawCellBorder(graphics, cellRect);
                graphics.DrawString(_properties[i].Name, headerFont, XBrushes.Black,
                    new XRect(xPos + CellPadding, yPos, colWidths[i + 1] - CellPadding, 20), XStringFormats.CenterLeft);
                xPos += colWidths[i + 1];
            }
            yPos += 20;
        }

        // Draw table rows
        for (int i = startIndex; i < endIndex; i++)
        {
            double xPos = MarginLeft;
            var rowNumber = i + 1;
            var item = _data[i];

            // Row number cell
            var cellRect = new XRect(xPos, yPos, colWidths[0], 20);
            DrawCellBorder(graphics, cellRect);
            graphics.DrawString(rowNumber.ToString(), cellFont, XBrushes.Black,
                new XRect(xPos + CellPadding, yPos, colWidths[0] - CellPadding, 20), XStringFormats.CenterLeft);
            xPos += colWidths[0];

            // Data cells
            for (int j = 0; j < _properties.Count; j++)
            {
                cellRect = new XRect(xPos, yPos, colWidths[j + 1], 20);
                DrawCellBorder(graphics, cellRect);
                var value = _properties[j].GetValue(item)?.ToString() ?? string.Empty;
                graphics.DrawString(value, cellFont, XBrushes.Black,
                    new XRect(xPos + CellPadding, yPos, colWidths[j + 1] - CellPadding, 20), XStringFormats.CenterLeft);
                xPos += colWidths[j + 1];
            }
            yPos += 20;
        }

        return yPos;
    }

    private double AddDisclaimer(XGraphics graphics, double yPos)
    {
        var textFont = new XFont("Arial", 8, XFontStyle.Regular);
        var boldFont = new XFont("Arial", 8, XFontStyle.Bold);

        double startY = yPos;

        // Disclaimer lines
        foreach (var line in _options.DisclaimerLines)
        {
            DrawFormattedText(graphics, line, MarginLeft, yPos, textFont, boldFont, _tableWidth);
            yPos += 14;
        }

        // Draw grid box around disclaimer content (matches table width exactly)
        double disclaimerHeight = yPos - startY;
        var gridRect = new XRect(MarginLeft, startY, _tableWidth, disclaimerHeight);
        graphics.DrawRectangle(new XPen(XColors.Gray, 1), gridRect);

        return yPos;
    }

    private void AddPageNumber(XGraphics graphics, int pageNum, int totalPages)
    {
        var font = new XFont("Arial", 9, XFontStyle.Regular);
        var text = $"page {pageNum} of {totalPages}";
        var width = graphics.MeasureString(text, font).Width;
        graphics.DrawString(text, font, XBrushes.Gray,
            new XRect((PageWidth - width) / 2, PageHeight - MarginBottom + 10, width, 20),
            XStringFormats.TopLeft);
    }

    private double[] CalculateColumnWidths()
    {
        // Create a temporary graphics context for measuring text
        using var tempDoc = new PdfDocument();
        var tempPage = tempDoc.AddPage();
        tempPage.Width = PageWidth;
        tempPage.Height = PageHeight;
        using var graphics = XGraphics.FromPdfPage(tempPage);

        var font = new XFont("Arial", _options.TableFontSize, XFontStyle.Bold);
        var headerFont = new XFont("Arial", _options.TableFontSize, XFontStyle.Bold);
        var cellFont = new XFont("Arial", _options.TableFontSize, XFontStyle.Regular);

        var widths = new double[_properties.Count + 1];

        // Row number column
        widths[0] = Math.Max(50, graphics.MeasureString("Row #", headerFont).Width + 20);

        // Measure each column's content
        var contentWidths = new List<double>();

        for (int i = 0; i < _properties.Count; i++)
        {
            double maxWidth = 0;

            // Measure header
            var headerSize = graphics.MeasureString(_properties[i].Name, headerFont);
            maxWidth = Math.Max(maxWidth, headerSize.Width);

            // Measure sample data (check first 50 rows for performance)
            int sampleSize = Math.Min(50, _data.Count);
            for (int j = 0; j < sampleSize; j++)
            {
                var value = _properties[i].GetValue(_data[j])?.ToString() ?? string.Empty;
                var valueSize = graphics.MeasureString(value, cellFont);
                maxWidth = Math.Max(maxWidth, valueSize.Width);
            }

            // Add padding
            maxWidth += 16;
            contentWidths.Add(maxWidth);
        }

        // Calculate total width needed
        double totalWidth = widths[0] + contentWidths.Sum();

        // If total width exceeds available width, scale down proportionally
        if (totalWidth > ContentWidth)
        {
            double scale = (ContentWidth - widths[0]) / contentWidths.Sum();
            for (int i = 0; i < contentWidths.Count; i++)
            {
                widths[i + 1] = Math.Max(30, contentWidths[i] * scale);
            }
        }
        else
        {
            // Distribute extra space proportionally
            double extraSpace = ContentWidth - totalWidth;
            for (int i = 0; i < contentWidths.Count; i++)
            {
                double extraPortion = extraSpace * (contentWidths[i] / contentWidths.Sum());
                widths[i + 1] = contentWidths[i] + extraPortion;
            }
        }

        return widths;
    }

    private void DrawCellBorder(XGraphics graphics, XRect rect)
    {
        var pen = new XPen(XColors.Gray, 1);
        graphics.DrawRectangle(pen, rect);
    }

    private void DrawFormattedText(XGraphics graphics, string text, double x, double y,
        XFont normalFont, XFont boldFont, double maxWidth)
    {
        // Parse for **bold** markup
        var remaining = text;
        double currentX = x;

        while (remaining.Length > 0)
        {
            var boldStart = remaining.IndexOf("**");
            if (boldStart < 0)
            {
                graphics.DrawString(remaining, normalFont, XBrushes.Black,
                    new XRect(currentX, y, maxWidth - (currentX - x), 20),
                    XStringFormats.TopLeft);
                break;
            }

            if (boldStart > 0)
            {
                var normalText = remaining.Substring(0, boldStart);
                var size = graphics.MeasureString(normalText, normalFont);
                graphics.DrawString(normalText, normalFont, XBrushes.Black,
                    new XRect(currentX, y, maxWidth, 20), XStringFormats.TopLeft);
                currentX += size.Width;
                remaining = remaining.Substring(boldStart);
            }

            var boldEnd = remaining.IndexOf("**", 2);
            if (boldEnd < 0)
            {
                graphics.DrawString(remaining, normalFont, XBrushes.Black,
                    new XRect(currentX, y, maxWidth - (currentX - x), 20),
                    XStringFormats.TopLeft);
                break;
            }

            var boldText = remaining.Substring(2, boldEnd - 2);
            var boldSize = graphics.MeasureString(boldText, boldFont);
            graphics.DrawString(boldText, boldFont, XBrushes.Black,
                new XRect(currentX, y, maxWidth, 20), XStringFormats.TopLeft);
            currentX += boldSize.Width;
            remaining = remaining.Substring(boldEnd + 2);
        }
    }

    private double startYPos => MarginTop + 100; // Approximate header height
}
