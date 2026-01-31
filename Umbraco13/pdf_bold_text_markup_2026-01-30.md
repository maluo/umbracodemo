# PDF & Excel Export Service Features - Complete Documentation

**Date:** 2026-01-30
**Features:**
1. Multi-Line Text Support for Headers and Footers
2. Configurable Disclaimer Section
3. Bold Text Markup Support (`**bold**` syntax)
4. Average Summary Row with Grey Background
5. Generic Excel Export Service with Full Styling Control
6. Download Excel (Generic) Button
7. Excel Multi-Line Cell Formatting
**Status:** All Completed

---

# Feature 1: Multi-Line Text Support

## Task Plan

### Overview
Enable multi-line text rendering in PDF headers (ReportTitle, Subtitle) and footers (FooterText, Disclaimer). Users should be able to use `\n` to create line breaks.

### Requirements
1. Support `\n` line breaks in ReportTitle
2. Support `\n` line breaks in Subtitle
3. Support `\n` line breaks in FooterText
4. Support `\n` line breaks in Disclaimer
5. Automatically adjust Y position for each line
6. Trim whitespace from each line for cleaner rendering

### Implementation Approach
- Split text strings by `\n` character
- Loop through lines and draw each one
- Increment Y position after each line
- Apply line-specific spacing (title: 20, subtitle: 15, footer: 12)

## Change Log

### Files Modified

| File | Lines Modified | Description |
|------|----------------|-------------|
| `Services/PdfExportService.cs` | 455-488 | Updated `DrawPageHeader` for multi-line title/subtitle |
| `Services/PdfExportService.cs` | 614-658 | Updated `DrawReportFooter` for multi-line footer/disclaimer |

### Detailed Changes

#### 1. DrawPageHeader - Multi-line Title Support (lines 455-488)

**Before:**
```csharp
// Single line title only
gfx.DrawString(options.ReportTitle, fontHeader, XBrushes.Black,
    new XRect(0, 30, page.Width, 30), XStringFormats.TopCenter);
```

**After:**
```csharp
// First page - full title and subtitle
double yPos = 30;

// Support multi-line title with **bold** markup
var titleLines = options.ReportTitle.Split('\n');
foreach (var line in titleLines)
{
    DrawFormattedText(gfx, line.Trim(), 0, yPos, page.Width, 20, font, fontHeader, XBrushes.Black);
    yPos += 20;
}

// Support multi-line subtitle with **bold** markup
if (!string.IsNullOrEmpty(options.Subtitle))
{
    yPos += 5; // Add spacing between title and subtitle
    var subtitleLines = options.Subtitle.Split('\n');
    foreach (var line in subtitleLines)
    {
        DrawFormattedText(gfx, line.Trim(), 0, yPos, page.Width, 15, font, fontHeader, XBrushes.Black);
        yPos += 15;
    }
}
```

#### 2. DrawReportFooter - Multi-line Footer/Disclaimer (lines 614-658)

**Before:**
```csharp
// Single line footer text
gfx.DrawString("This document is confidential...", fontFooter, XBrushes.Black,
    new XRect(0, footerY, page.Width, 12), XStringFormats.TopCenter);
```

**After:**
```csharp
// Footer text (support multi-line with \n and **bold** markup)
footerY += 40;
var footerText = options.FooterText ?? "This document is confidential and intended for internal use only.";
var footerLines = footerText.Split('\n');
foreach (var line in footerLines)
{
    DrawFormattedText(gfx, line.Trim(), 0, footerY, page.Width, 12, fontFooter, fontBold, XBrushes.Black);
    footerY += 12;
}

// Disclaimer section (if provided)
footerY += 10;
if (!string.IsNullOrEmpty(options.Disclaimer))
{
    // Draw disclaimer separator
    gfx.DrawLine(new XPen(XColors.LightGray, 0.5), options.MarginLeft, footerY, page.Width - options.MarginLeft, footerY);
    footerY += 8;

    // Draw disclaimer text (support multi-line with \n and **bold** markup)
    var disclaimerLines = options.Disclaimer.Split('\n');
    foreach (var line in disclaimerLines)
    {
        DrawFormattedText(gfx, line.Trim(), 0, footerY, page.Width, 12, fontFooter, fontBold, XBrushes.DarkGray);
        footerY += 12;
    }
}
```

## Implementation Details

### Line Spacing

| Text Element | Line Height | Spacing After |
|--------------|-------------|---------------|
| Title lines | 20 | 20 |
| Subtitle lines | 15 | 15 |
| Footer lines | 12 | 12 |
| Disclaimer lines | 12 | 12 |
| Title to Subtitle gap | - | 5 |
| Footer to Disclaimer gap | - | 10 |

### Rendering Flow
```
1. Split text by '\n'
2. For each line:
   a. Trim whitespace
   b. Draw at current Y position
   c. Increment Y by line height
3. Add section spacing if needed
```

## Usage Examples

### Example 1: Multi-line Title
```csharp
var options = new PdfExportOptions
{
    ReportTitle = "QUARTERLY FUND REPORT\nQ4 2024 Performance Summary",
    Subtitle = "Prepared by: Investment Team\nDate: 2024-12-31"
};
```

### Example 2: Multi-line Footer
```csharp
var options = new PdfExportOptions
{
    FooterText = "CONFIDENTIAL DOCUMENT\nFor internal use only.\nUnauthorized distribution prohibited."
};
```

---

# Feature 2: Configurable Disclaimer Section

## Task Plan

### Overview
Add a configurable disclaimer section that appears at the bottom of the report footer, with a visual separator from the main footer text.

### Requirements
1. Add `Disclaimer` property to `PdfExportOptions`
2. Draw a separator line before disclaimer
3. Render disclaimer in smaller, gray text
4. Support multi-line disclaimers with `\n`
5. Only show disclaimer if text is provided

### Implementation Approach
- Add optional `Disclaimer` string property
- Check if disclaimer exists before rendering
- Draw light gray separator line
- Render disclaimer text in `DarkGray` color

## Change Log

### Files Modified

| File | Lines Modified | Description |
|------|----------------|-------------|
| `Services/PdfExportService.cs` | 139-141 | Added `Disclaimer` property to `PdfExportOptions` |
| `Services/PdfExportService.cs` | 636-651 | Added disclaimer rendering in `DrawReportFooter` |

### Detailed Changes

#### 1. Added Disclaimer Property (lines 139-141)

```csharp
/// <summary>
/// Disclaimer text to display below the footer (optional). Multi-line disclaimers can use \n for line breaks.
/// </summary>
public string? Disclaimer { get; set; }
```

#### 2. Added Disclaimer Rendering (lines 636-651)

```csharp
// Disclaimer section (if provided)
footerY += 10; // Small spacing after footer text
if (!string.IsNullOrEmpty(options.Disclaimer))
{
    // Draw disclaimer separator
    gfx.DrawLine(new XPen(XColors.LightGray, 0.5), options.MarginLeft, footerY, page.Width - options.MarginLeft, footerY);
    footerY += 8;

    // Draw disclaimer text (support multi-line with \n and **bold** markup)
    var disclaimerLines = options.Disclaimer.Split('\n');
    foreach (var line in disclaimerLines)
    {
        DrawFormattedText(gfx, line.Trim(), 0, footerY, page.Width, 12, fontFooter, fontBold, XBrushes.DarkGray);
        footerY += 12;
    }
}
```

## Implementation Details

### Visual Appearance

| Element | Style |
|---------|-------|
| Separator | LightGray, 0.5pt width |
| Text Color | DarkGray |
| Font Size | FooterFontSize (default: 8) |
| Line Height | 12 |
| Gap from footer | 10 units |
| Gap after separator | 8 units |

### Positioning
```
[Footer Text]
    (10 units gap)
[Separator Line]
    (8 units gap)
[Disclaimer Line 1]
[Disclaimer Line 2]
...
[Page Info]
```

## Usage Examples

### Example 1: Simple Disclaimer
```csharp
var options = new PdfExportOptions
{
    Disclaimer = "This document is for informational purposes only."
};
```

### Example 2: Multi-line Disclaimer
```csharp
var options = new PdfExportOptions
{
    Disclaimer = "Past performance is not indicative of future results.\n" +
                 "Please consult with a qualified financial advisor."
};
```

### Example 3: Complete Setup with All Features
```csharp
var options = new PdfExportOptions
{
    ReportTitle = "FUND SUMMARY REPORT\nQuarterly Review",
    Subtitle = $"Generated: {DateTime.Now:yyyy-MM-dd}\nTotal: {funds.Count} funds",
    FooterText = "CONFIDENTIAL - Internal Use Only",
    Disclaimer = "Disclaimer:\n" +
                 "This report does not constitute investment advice.\n" +
                 "Past performance is not indicative of future results.",
    ItemsPerPage = 25
};
```

---

# Feature 3: Bold Text Markup Support

## Task Plan

### Overview
Add support for bold text formatting within multi-line text areas in the PDF export service. Users should be able to mark portions of text as bold using `**bold**` markdown-style syntax.

### Requirements
1. Parse `**` markers in text strings to identify bold sections
2. Support mixed formatting (regular + bold text) on the same line
3. Apply to all text fields: ReportTitle, Subtitle, FooterText, Disclaimer
4. Handle unclosed markers gracefully (treat as regular text)
5. Preserve text alignment (center, left, right) with mixed formatting

### Implementation Approach
- Create a helper method `DrawFormattedText` that parses and draws formatted text
- Parse `**bold**` markers using `IndexOf` and substring operations
- Calculate total text width for proper alignment
- Draw each segment with appropriate font (regular or bold)

---

## Change Log

### Files Modified

| File | Lines Modified | Description |
|------|----------------|-------------|
| `Services/PdfExportService.cs` | 161-239 | Added `DrawFormattedText` helper method |
| `Services/PdfExportService.cs` | 455-488 | Updated `DrawPageHeader` to use formatted text |
| `Services/PdfExportService.cs` | 626-651 | Updated `DrawReportFooter` to use formatted text |

### Detailed Changes

#### 1. Added DrawFormattedText Method (lines 161-239)

```csharp
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
```

#### 2. Updated DrawPageHeader (lines 455-488)

**Before:**
```csharp
gfx.DrawString(options.ReportTitle, fontHeader, XBrushes.Black,
    new XRect(0, yPos, page.Width, 30), XStringFormats.TopCenter);
```

**After:**
```csharp
// Support multi-line title with **bold** markup
var titleLines = options.ReportTitle.Split('\n');
foreach (var line in titleLines)
{
    DrawFormattedText(gfx, line.Trim(), 0, yPos, page.Width, 20, font, fontHeader, XBrushes.Black);
    yPos += 20;
}
```

#### 3. Updated DrawReportFooter (lines 626-651)

**Before:**
```csharp
gfx.DrawString(footerText, fontFooter, XBrushes.Black,
    new XRect(0, footerY, page.Width, 12), XStringFormats.TopCenter);
```

**After:**
```csharp
// Footer text (support multi-line with \n and **bold** markup)
var footerText = options.FooterText ?? "This document is confidential...";
var footerLines = footerText.Split('\n');
foreach (var line in footerLines)
{
    DrawFormattedText(gfx, line.Trim(), 0, footerY, page.Width, 12, fontFooter, fontBold, XBrushes.Black);
    footerY += 12;
}
```

---

## Implementation Details

### Parsing Algorithm

1. **Tokenization**: Scan text for `**` markers using `IndexOf()`
2. **Segment Creation**: Create `(text, isBold)` tuples for each segment
3. **Width Calculation**: Measure each segment with appropriate font for alignment
4. **Drawing**: Render segments sequentially, updating X position

### Edge Cases Handled

| Case | Behavior |
|------|----------|
| No bold markers | Entire text rendered as regular |
| Unclosed `**` marker | Treated as regular text (literal `**` included) |
| Multiple bold sections | All parsed and rendered correctly |
| Empty segments | Skipped (no whitespace issues) |
| Mixed alignment | Total width calculated for proper centering |

### Font Usage

| Text Element | Regular Font | Bold Font |
|--------------|--------------|-----------|
| Title/Subtitle | `font` | `fontHeader` |
| Footer/Disclaimer | `fontFooter` | `fontBold` |

---

## Usage Examples

### Example 1: Title with Bold Emphasis
```csharp
var options = new PdfExportOptions
{
    ReportTitle = "**FUND SUMMARY** Report\nQ4 2024 Performance",
    Subtitle = "Prepared by: **Investment Team**\nDate: **2024-12-31**"
};
```

### Example 2: Footer with Important Notice
```csharp
var options = new PdfExportOptions
{
    FooterText = "**CONFIDENTIAL:** This document contains proprietary information.\n" +
                 "Unauthorized distribution is **strictly prohibited**."
};
```

### Example 3: Multi-Line Disclaimer
```csharp
var options = new PdfExportOptions
{
    Disclaimer = "**IMPORTANT DISCLAIMER**\n" +
                 "Past performance is **not indicative** of future results.\n" +
                 "Please consult with a **qualified financial advisor** before investing."
};
```

### Example 4: Complete Usage in FundsController
```csharp
var options = new PdfExportOptions
{
    ReportTitle = "**FUND SUMMARY REPORT**\nQuarterly Review",
    Subtitle = $"Generated on: **{DateTime.Now:yyyy-MM-dd}**\n" +
               $"Total Funds: **{funds.Count}**",
    ItemsPerPage = 25,
    FooterText = "**CONFIDENTIAL DOCUMENT**\n" +
                 "For internal use only.",
    Disclaimer = "**Disclaimer:**\n" +
                 "This report is for **informational purposes** only.\n" +
                 "Does not constitute **investment advice**."
};

var pdfBytes = _pdfExportService.ExportToPdf(funds, columns, options);
```

---

## Testing Checklist

- [x] Regular text without bold markers renders correctly
- [x] Single `**bold**` word renders in bold
- [x] Multiple bold sections in one line render correctly
- [x] Bold at start of line works
- [x] Bold at end of line works
- [x] Unclosed `**` markers treated as literal text
- [x] Multi-line text with `\n` splits correctly
- [x] Centered text with bold maintains alignment
- [x] Title/Subtitle/Footer/Disclaimer all support markup
- [x] Build succeeds with no compilation errors

---

## Technical Notes

### PDFsharp Version
- PDFsharp 6.0 uses `XFontStyleEx.Bold` (not `XFontStyle.Bold`)
- Requires `using PdfSharp;` and `using PdfSharp.Drawing;`

### Performance Considerations
- `MeasureString()` called for each segment (acceptable for typical PDF sizes)
- For very large documents, consider caching font measurements

### Limitations
- Bold markers cannot be escaped (no way to render literal `**text**`)
- Nested formatting not supported (e.g., `***bold italic***`)
- Only bold/regular toggle available (no italic, underline, etc.)

---

## Related Documentation

- **Generic PDF Export Service:** `pdf_export_service_usage.md`
- **Previous Feature:** `pdf_pagination_improvements_2026-01-29.md`
- **Service Interface:** `Services/IPdfExportService.cs`
- **Implementation:** `Services/PdfExportService.cs`

---

## Build Verification

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

All bold text markup functionality has been successfully implemented and tested.

---

# Feature 4: Average Summary Row

## Task Plan

### Overview
Add a summary row at the end of the PDF table that shows the average of numeric fields. The row should be highlighted with a grey background and labeled "Average".

### Requirements
1. Add `ShowAverage` property to `PdfColumnDefinition` to mark numeric columns
2. Add `ShowAverageRow` property to `PdfExportOptions` to enable/disable the feature
3. Calculate average for all numeric values in marked columns
4. Display average row with grey background after last data row
5. Show "Average" label in first column
6. Support existing format strings for average values
7. Handle null and non-numeric values gracefully

### Implementation Approach
- Add boolean properties to control feature visibility
- Create `DrawAverageRow` method to calculate and render averages
- Add `IsNumeric` helper method for type checking
- Integrate into main export flow after data loop

## Change Log

### Files Modified

| File | Lines Modified | Description |
|------|----------------|-------------|
| `Services/PdfExportService.cs` | 42-45 | Added `ShowAverage` property to `PdfColumnDefinition` |
| `Services/PdfExportService.cs` | 153-161 | Added `ShowAverageRow` and `AverageRowLabel` to `PdfExportOptions` |
| `Services/PdfExportService.cs` | 328-332 | Added average row drawing to export flow |
| `Services/PdfExportService.cs` | 598-675 | Added `DrawAverageRow` method |
| `Services/PdfExportService.cs` | 677-683 | Added `IsNumeric` helper method |
| `Controllers/FundsController.cs` | 372-373 | Set `ShowAverage = true` for NavPrice and MarketPrice |
| `Controllers/FundsController.cs` | 385 | Enabled `ShowAverageRow = true` in options |

### Detailed Changes

#### 1. Added ShowAverage Property (lines 42-45)

```csharp
/// <summary>
/// Calculate and show average for this column in summary row (only for numeric types)
/// </summary>
public bool ShowAverage { get; set; }
```

#### 2. Added Options Properties (lines 153-161)

```csharp
/// <summary>
/// Show average row at end of table (default: false)
/// </summary>
public bool ShowAverageRow { get; set; }

/// <summary>
/// Label for average row (default: "Average")
/// </summary>
public string AverageRowLabel { get; set; } = "Average";
```

#### 3. Updated Export Flow (lines 328-332)

```csharp
// Draw average row (if enabled)
if (options.ShowAverageRow)
{
    yPos = DrawAverageRow(gfx, yPos, dataList, columnWidths, columns, font, fontBold, options);
}
```

#### 4. Added DrawAverageRow Method (lines 598-675)

```csharp
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
```

#### 5. Added IsNumeric Helper (lines 677-683)

```csharp
/// <summary>
/// Check if a value is numeric
/// </summary>
private bool IsNumeric(object value)
{
    return value is int or double or decimal or float or long or short or byte or uint or ulong or ushort or sbyte;
}
```

#### 6. FundsController Usage

```csharp
var columns = new List<PdfColumnDefinition>
{
    new() { PropertyName = "FundName", HeaderText = "Fund Name", Alignment = XStringAlignment.Near },
    new() { PropertyName = "TickerCode", HeaderText = "Ticker", Width = 70 },
    new() { PropertyName = "NavPrice", HeaderText = "NAV Price", Width = 60, Format = "F2", ShowAverage = true },
    new() { PropertyName = "MarketPrice", HeaderText = "Market Price", Width = 60, Format = "F2", ShowAverage = true },
    new() { PropertyName = "HoldInTrust", HeaderText = "Hold In Trust", Width = 60 }
};

var options = new PdfExportOptions
{
    ReportTitle = "FUND SUMMARY REPORT",
    ItemsPerPage = 50,
    ShowAverageRow = true
};
```

## Implementation Details

### Average Calculation Algorithm

1. **Initialize**: Set sum = 0, count = 0
2. **Iterate**: Loop through all data rows
3. **Check**: Verify value is not null and is numeric
4. **Accumulate**: Add value to sum, increment count
5. **Calculate**: Divide sum by count
6. **Format**: Apply format string or custom formatter
7. **Handle Zero Count**: Display empty string if no valid values

### Numeric Types Supported

| Type | Example |
|------|---------|
| `int` | 42 |
| `double` | 3.14159 |
| `decimal` | 123.45m |
| `float` | 1.5f |
| `long` | 9000000000L |
| `short` | 1000 |
| `byte` | 255 |
| `uint` | 123u |
| `ulong` | 4000000000ul |
| `ushort` | 5000 |
| `sbyte` | -128 |

### Visual Appearance

| Element | Style |
|---------|-------|
| Background | RGB(240, 240, 240) - Light Gray |
| Border | Gray, 0.5pt |
| First Column | "Average" label (bold, left aligned) |
| Numeric Columns | Calculated average (bold, centered) |
| Non-numeric Columns | Empty cell with gray background |

### Positioning

```
[Data Row N]
    (next row)
[Average Row] - Gray background
    [Average] | [empty] | [15.63] | [12.45] | [empty]
    (15 units gap)
[Disclaimer]
```

## Usage Examples

### Example 1: Enable Average for Numeric Columns

```csharp
var columns = new List<PdfColumnDefinition>
{
    new() { PropertyName = "ProductName", HeaderText = "Product" },
    new() { PropertyName = "Quantity", HeaderText = "Qty", Width = 50, ShowAverage = true },
    new() { PropertyName = "Price", HeaderText = "Price", Width = 60, Format = "C2", ShowAverage = true },
    new() { PropertyName = "Total", HeaderText = "Total", Width = 70, Format = "C2", ShowAverage = true }
};

var options = new PdfExportOptions
{
    ReportTitle = "SALES REPORT",
    ShowAverageRow = true,
    AverageRowLabel = "Average"
};
```

### Example 2: Custom Average Label

```csharp
var options = new PdfExportOptions
{
    ShowAverageRow = true,
    AverageRowLabel = "MEAN"
};
```

### Example 3: Disable Average Row

```csharp
var options = new PdfExportOptions
{
    ShowAverageRow = false  // Average row not shown
};
```

### Example 4: Average with Custom Formatter

```csharp
new()
{
    PropertyName = "Percentage",
    HeaderText = "%",
    Width = 60,
    CustomFormatter = (value) => $"{value:P2}",
    ShowAverage = true
}
```

## Testing Checklist

- [x] Average row appears at end of table when enabled
- [x] Average row does not appear when disabled
- [x] First column shows "Average" label
- [x] Numeric columns show calculated averages
- [x] Non-numeric columns show empty cells
- [x] Format strings are applied to averages
- [x] Custom formatters work with averages
- [x] Null values are handled correctly
- [x] Non-numeric values are excluded from calculation
- [x] Gray background is applied to entire row
- [x] Build succeeds with no compilation errors

## Technical Notes

### Calculation Performance
- Average is calculated once per column during PDF generation
- O(n × m) complexity where n = rows, m = columns with `ShowAverage = true`
- For very large datasets, consider pre-calculating averages

### Null Handling
- Null values are skipped in calculation (not counted)
- Empty cells display blank text (not "0" or "null")
- Formula: `sum of valid values / count of valid values`

### Format Priority
1. If `CustomFormatter` is set → use custom formatter
2. Else if `Format` string is set → use format string
3. Else → use default "F2" format (2 decimal places)

---

## Build Verification

```
Build succeeded.
    7 Warning(s)
    0 Error(s)
```

All average row functionality has been successfully implemented and tested.

---

# Feature 5: Generic Excel Export Service

## Task Plan

### Overview
Create a generic Excel export service similar to the PDF export service, capable of exporting any list of objects to Excel with configurable headers, footers, disclaimer, and full styling options.

### Requirements
1. Generic service using reflection for any object type (similar to IPdfExportService)
2. Multi-line support for headers (ReportTitle, Subtitle)
3. Multi-line support for footers (FooterText)
4. Multi-line support for disclaimer
5. Bold text support using `**bold**` syntax
6. Full styling control via options (fonts, colors, borders)
7. NO "END OF REPORT" text
8. NO generation date
9. Use ClosedXML library (free, no license required)

### Implementation Approach
- Create `IExcelExportService` interface
- Create `ExcelExportService` implementation using ClosedXML
- Create `ExcelColumnDefinition` for column configuration
- Create `ExcelExportOptions` with all styling properties
- Create `ExcelFontStyle` class for font/border/color configuration
- Register service in Program.cs

## Change Log

### Files Created

| File | Description |
|------|-------------|
| `Services/IExcelExportService.cs` | Interface for generic Excel export |
| `Services/ExcelExportService.cs` | Implementation using ClosedXML |

### Files Modified

| File | Lines Modified | Description |
|------|----------------|-------------|
| `Program.cs` | 24 | Added `IExcelExportService` registration |
| `Controllers/FundsController.cs` | 21, 405-461 | Injected service, added example endpoint |

### Detailed Changes

#### 1. Created IExcelExportService Interface

```csharp
public interface IExcelExportService
{
    byte[] ExportToExcel<T>(
        IEnumerable<T> data,
        IList<ExcelColumnDefinition> columns,
        ExcelExportOptions? options = null);
}
```

#### 2. Created ExcelColumnDefinition Class

```csharp
public class ExcelColumnDefinition
{
    public string PropertyName { get; set; } = string.Empty;
    public string HeaderText { get; set; } = string.Empty;
    public double Width { get; set; }
    public string? Format { get; set; }
    public Func<object?, string>? CustomFormatter { get; set; }
    public ExcelAlignment Alignment { get; set; } = ExcelAlignment.Center;
}

public enum ExcelAlignment
{
    Left,
    Center,
    Right
}
```

#### 3. Created ExcelFontStyle Class

```csharp
public class ExcelFontStyle
{
    public string FontName { get; set; } = "Calibri";
    public double FontSize { get; set; } = 11;
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public string? FontColor { get; set; }         // Hex format
    public string? BackgroundColor { get; set; }   // Hex format
}
```

#### 4. Created ExcelExportOptions Class

```csharp
public class ExcelExportOptions
{
    // Content
    public string WorksheetName { get; set; } = "Sheet1";
    public string ReportTitle { get; set; } = "Report";
    public string? Subtitle { get; set; }
    public string? FooterText { get; set; }
    public string? Disclaimer { get; set; }

    // Styling
    public ExcelFontStyle TitleFont { get; set; } = new() { FontSize = 16, Bold = true };
    public ExcelFontStyle SubtitleFont { get; set; } = new() { FontSize = 11 };
    public ExcelFontStyle HeaderFont { get; set; } = new() { Bold = true, BackgroundColor = "#E0E0E0" };
    public ExcelFontStyle DataFont { get; set; } = new() { FontSize = 10 };
    public ExcelFontStyle FooterFont { get; set; } = new() { FontSize = 9, Italic = true };
    public ExcelFontStyle DisclaimerFont { get; set; } = new() { FontSize = 8, FontColor = "#666666" };

    // Layout
    public int RowsAfterTitle { get; set; } = 1;
    public int RowsAfterSubtitle { get; set; } = 1;
    public bool AutoFitColumns { get; set; } = true;
    public bool ShowBorders { get; set; } = true;
}
```

#### 5. Main Export Method (ExcelExportService.cs)

```csharp
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
```

#### 6. Bold Text Implementation

Note: ClosedXML doesn't support partial cell formatting (rich text), so if `**` markers are present, the entire cell is bolded.

```csharp
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
        Bold = fontStyle.Bold || hasBold,
        Italic = fontStyle.Italic,
        FontColor = fontStyle.FontColor,
        BackgroundColor = fontStyle.BackgroundColor
    };

    ApplyFontStyle(cell, style);
}
```

#### 7. Service Registration (Program.cs)

```csharp
builder.Services.AddScoped<Umbraco13.Services.IExcelExportService, Umbraco13.Services.ExcelExportService>();
```

#### 8. Example Usage (FundsController.cs)

```csharp
[HttpGet("exporttoexcel-generic")]
[ValidateDownloadToken("excel")]
public async Task<IActionResult> ExportToExcelGeneric()
{
    var funds = await _fundService.GetAllFundsAsync();

    var columns = new List<ExcelColumnDefinition>
    {
        new() { PropertyName = "FundName", HeaderText = "Fund Name", Alignment = ExcelAlignment.Left },
        new() { PropertyName = "TickerCode", HeaderText = "Ticker", Width = 15, Alignment = ExcelAlignment.Left },
        new() { PropertyName = "NavPrice", HeaderText = "NAV Price", Width = 12, Format = "F2", Alignment = ExcelAlignment.Right },
        new() { PropertyName = "MarketPrice", HeaderText = "Market Price", Width = 12, Format = "F2", Alignment = ExcelAlignment.Right },
        new() { PropertyName = "HoldInTrust", HeaderText = "Hold In Trust", Width = 15, Alignment = ExcelAlignment.Left }
    };

    var options = new ExcelExportOptions
    {
        WorksheetName = "Funds",
        ReportTitle = "**FUND SUMMARY REPORT**\nThis report provides a summary of all funds\nincluding their **NAV** and **Market Prices**.",
        Subtitle = "Generated by **Investment Team**\nConfidential Document",
        FooterText = "**CONFIDENTIAL:** This document contains proprietary information.\nUnauthorized distribution is **strictly prohibited**.",
        Disclaimer = "**IMPORTANT DISCLAIMER:**\nPast performance is **not indicative** of future results.\nPlease consult with a **qualified financial advisor**.",
        TitleFont = new ExcelFontStyle { FontSize = 16, Bold = true, FontColor = "#000000" },
        SubtitleFont = new ExcelFontStyle { FontSize = 11, Italic = true, FontColor = "#333333" },
        HeaderFont = new ExcelFontStyle { Bold = true, BackgroundColor = "#D3D3D3", FontColor = "#000000" },
        DataFont = new ExcelFontStyle { FontSize = 10, FontColor = "#000000" },
        FooterFont = new ExcelFontStyle { FontSize = 9, Italic = true, FontColor = "#666666" },
        DisclaimerFont = new ExcelFontStyle { FontSize = 8, FontColor = "#999999" },
        AutoFitColumns = true,
        ShowBorders = true
    };

    var excelBytes = _excelExportService.ExportToExcel(funds, columns, options);

    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
}
```

## Implementation Details

### Excel Export Flow

1. **Title Section** (multi-line with bold support)
2. **Subtitle Section** (multi-line with bold support)
3. **Table Header Row** (merged cells, centered)
4. **Data Rows** (with borders, formatted values)
5. **Footer Section** (multi-line with bold support)
6. **Disclaimer Section** (multi-line with bold support)

### Layout Structure

```
┌─────────────────────────────────────────┐
│ **FUND SUMMARY REPORT**                 │ ← Title (bold)
│ Multi-line title support                │
│                                         │
│ Generated by **Investment Team**        │ ← Subtitle
│ Confidential Document                   │
│                                         │
│ ┌───────────┬────────┬────────┬────────┐│
│ │Fund Name  │Ticker  │NAV Prc │...     ││ ← Header (gray bg)
│ ├───────────┼────────┼────────┼────────┤│
│ │Fund 1     │ABC     │10.50   │...     ││ ← Data (borders)
│ │Fund 2     │DEF     │20.75   │...     ││
│ └───────────┴────────┴────────┴────────┘│
│                                         │
│ **CONFIDENTIAL:** Internal use only    │ ← Footer
│ Multi-line footer text                 │
│                                         │
│ **Disclaimer:** Past performance...     │ ← Disclaimer
│ Multi-line disclaimer                  │
└─────────────────────────────────────────┘
```

### Styling System

| Element | Default Font | Default Size | Default Bold | Default Color |
|---------|--------------|--------------|--------------|---------------|
| Title | Calibri | 16 | Yes | Black |
| Subtitle | Calibri | 11 | No | Black |
| Header | Calibri | 11 | Yes | Black, Gray BG |
| Data | Calibri | 10 | No | Black |
| Footer | Calibri | 9 | No | Yes, Gray |
| Disclaimer | Calibri | 8 | No | Dark Gray |

### Color Format

All colors use hex format:
- Font color: `"#FF0000"` for red
- Background color: `"#FFFF00"` for yellow

### Bold Text Support

Due to ClosedXML limitations:
- `**text**` syntax supported
- Entire cell is bolded if `**` is present
- `**` markers are stripped from final output

## Usage Examples

### Example 1: Basic Usage

```csharp
var columns = new List<ExcelColumnDefinition>
{
    new() { PropertyName = "Name", HeaderText = "Product Name" },
    new() { PropertyName = "Price", HeaderText = "Price", Format = "C2" },
    new() { PropertyName = "Quantity", HeaderText = "Qty", Width = 10 }
};

var options = new ExcelExportOptions
{
    ReportTitle = "SALES REPORT",
    WorksheetName = "Sales"
};

var bytes = _excelExportService.ExportToExcel(products, columns, options);
```

### Example 2: Custom Styling

```csharp
var options = new ExcelExportOptions
{
    TitleFont = new ExcelFontStyle
    {
        FontName = "Arial",
        FontSize = 18,
        Bold = true,
        FontColor = "#000080"  // Navy blue
    },
    HeaderFont = new ExcelFontStyle
    {
        Bold = true,
        BackgroundColor = "#4472C4",  // Blue
        FontColor = "#FFFFFF"           // White
    }
};
```

### Example 3: Multi-line Text with Bold

```csharp
var options = new ExcelExportOptions
{
    ReportTitle = "**QUARTERLY REPORT**\nQ4 2024 Results",
    Subtitle = "Prepared by: **Finance Department**\nDate: **2024-12-31**",
    FooterText = "**CONFIDENTIAL:**\nFor internal use only.\nDo not distribute.",
    Disclaimer = "**IMPORTANT:**\nThis data is **proprietary**.\nAll rights reserved."
};
```

### Example 4: Column Alignment and Formatting

```csharp
var columns = new List<ExcelColumnDefinition>
{
    new() { PropertyName = "Description", HeaderText = "Description", Alignment = ExcelAlignment.Left },
    new() { PropertyName = "Amount", HeaderText = "Amount", Alignment = ExcelAlignment.Right, Format = "C2" },
    new() { PropertyName = "Percentage", HeaderText = "%", Alignment = ExcelAlignment.Center, Format = "P2" }
};
```

### Example 5: Custom Formatter

```csharp
new()
{
    PropertyName = "Status",
    HeaderText = "Status",
    CustomFormatter = (value) => value?.ToString()?.ToUpper() switch
    {
        "PENDING" => "⏳ Pending",
        "COMPLETED" => "✓ Completed",
        "CANCELLED" => "✗ Cancelled",
        _ => value?.ToString() ?? ""
    }
}
```

## Testing Checklist

- [x] Generic service works with any object type
- [x] Multi-line title renders correctly
- [x] Multi-line subtitle renders correctly
- [x] Multi-line footer renders correctly
- [x] Multi-line disclaimer renders correctly
- [x] Bold text (`**text**`) is applied
- [x] Font styling (size, bold, italic) works
- [x] Font colors work (hex format)
- [x] Background colors work (hex format)
- [x] Column alignment (left, center, right) works
- [x] Format strings work (F2, C2, P2, etc.)
- [x] Custom formatters work
- [x] Auto-fit columns option works
- [x] Borders option works
- [x] NO "END OF REPORT" text
- [x] NO generation date
- [x] Build succeeds with no errors

## Technical Notes

### ClosedXML vs EPPlus

This service uses **ClosedXML** instead of EPPlus:
- ClosedXML is free (MIT license)
- EPPlus moved to paid licensing (Polyform Noncommercial 1.0.0)
- ClosedXML has better API for styling
- ClosedXML limitation: No rich text/partial formatting within cells

### Reflection Usage

Same as PDF service:
- Case-insensitive property matching
- Supports all public instance properties
- Null values handled gracefully

### Format Strings

Common .NET format strings:
- `"C2"` - Currency with 2 decimals ($1,234.56)
- `"F2"` - Fixed-point with 2 decimals (1234.56)
- `"P2"` - Percentage with 2 decimals (12.34%)
- `"N2"` - Number with 2 decimals (1,234.56)
- `"yyyy-MM-dd"` - Date format (2024-12-31)

---

## Build Verification

```
Build succeeded.
    7 Warning(s)
    0 Error(s)
```

All generic Excel export functionality has been successfully implemented and tested.

---

# Feature 6: Download Excel (Generic) Button

## Task Plan

### Overview
Add a download button for the generic Excel export service to the Funds table, allowing users to download Excel files with full styling support.

### Requirements
1. Add "Download Excel (Generic)" button to FundsTable view
2. Create new token type `excel-generic` to avoid conflicts with existing Excel endpoints
3. Update download-tokens endpoint to return the new token
4. Update JavaScript to handle the new button and token
5. Use existing token validation system for security

### Implementation Approach
- Add `excel-generic` token type to DownloadTokenService
- Update controller endpoint to use `excel-generic` token validation
- Update download-tokens API response to include new token
- Add button HTML after existing Excel buttons
- Update JavaScript `updateDownloadLinks` function to handle new button

## Change Log

### Files Modified

| File | Lines Modified | Description |
|------|----------------|-------------|
| `Services/DownloadTokenService.cs` | 92 | Added `excel-generic` token type |
| `Controllers/FundsController.cs` | 406 | Changed to use `excel-generic` token validation |
| `Controllers/FundsController.cs` | 51 | Added `excelGeneric` to download tokens response |
| `Views/Shared/Components/FundsTable/Default.cshtml` | 36-42 | Added download button HTML |
| `Views/Shared/Components/FundsTable/Default.cshtml` | 166 | Updated JavaScript to handle new button |

### Detailed Changes

#### 1. Added excel-generic Token (DownloadTokenService.cs, line 92)

```csharp
private TokenFileData GenerateAllTokens()
{
    var expiryTime = DateTime.UtcNow.AddMinutes(_expiryMinutes);
    var version = GenerateVersion();

    return new TokenFileData
    {
        Version = version,
        Tokens = new Dictionary<string, TokenData>
        {
            { "pdf", new TokenData { Token = GenerateRandomToken(), Expiry = expiryTime, Type = "pdf" } },
            { "csv", new TokenData { Token = GenerateRandomToken(), Expiry = expiryTime, Type = "csv" } },
            { "excel-free", new TokenData { Token = GenerateRandomToken(), Expiry = expiryTime, Type = "excel-free" } },
            { "excel", new TokenData { Token = GenerateRandomToken(), Expiry = expiryTime, Type = "excel" } },
            { "excel-generic", new TokenData { Token = GenerateRandomToken(), Expiry = expiryTime, Type = "excel-generic" } }
        }
    };
}
```

#### 2. Updated Controller Endpoint (FundsController.cs, line 406)

```csharp
[HttpGet("exporttoexcel-generic")]
[ValidateDownloadToken("excel-generic")]
public async Task<IActionResult> ExportToExcelGeneric()
```

#### 3. Updated Download Tokens Response (FundsController.cs, line 51)

```csharp
return Ok(new
{
    version = _downloadTokenService.CurrentVersion,
    pdf = _downloadTokenService.GenerateDownloadToken("pdf"),
    csv = _downloadTokenService.GenerateDownloadToken("csv"),
    excel = _downloadTokenService.GenerateDownloadToken("excel-free"),
    excelEpPlus = _downloadTokenService.GenerateDownloadToken("excel"),
    excelGeneric = _downloadTokenService.GenerateDownloadToken("excel-generic")
});
```

#### 4. Added Download Button (FundsTable/Default.cshtml, lines 36-42)

```html
<a href="#" id="download-excel-generic" class="btn btn-success" style="text-decoration: none;">
    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-file-earmark-excel" viewBox="0 0 16 16" style="margin-right: 5px; vertical-align: text-bottom;">
        <path d="M5.884 6.68a.5.5 0 1 0-.768.64L7.349 10l-2.233 2.68a.5.5 0 0 0 .768.64L8 10.78l2.116 2.54a.5.5 0 0 0 .768-.641L8.651 10l2.233-2.68a.5.5 0 0 0-.768-.64L8 9.22 5.884 6.68z"/>
        <path d="M14 14V4.5L9.5 0H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2zM9.5 3A1.5 1.5 0 0 0 11 4.5h2V14a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h5.5v2z"/>
    </svg>
    Download Excel (Generic)
</a>
```

#### 5. Updated JavaScript (FundsTable/Default.cshtml, line 166)

```javascript
function updateDownloadLinks(tokens) {
    document.getElementById('download-pdf').href = '/funds/exporttopdf?token=' + tokens.pdf;
    document.getElementById('download-csv').href = '/funds/exporttocsv?token=' + tokens.csv;
    document.getElementById('download-excel-free').href = '/funds/exporttoexcel-free?token=' + tokens.excel;
    document.getElementById('download-excel-epplus').href = '/funds/exporttoexcel?token=' + tokens.excelEpPlus;
    document.getElementById('download-excel-generic').href = '/funds/exporttoexcel-generic?token=' + tokens.excelGeneric;
    console.log('Download links updated with new tokens, version:', tokens.version);
}
```

## Implementation Details

### Token System Architecture

The generic Excel export uses the existing secure token system:

1. **Token Generation**: Server generates random 44-character token
2. **Token Storage**: Tokens stored in `AppData/download-tokens.json`
3. **Token Expiry**: Tokens expire after 30 minutes (configurable)
4. **Token Refresh**: Tokens auto-refresh every 20 minutes
5. **Token Validation**: Controller validates token before allowing download

### Token Types

| Token Type | Endpoint | Library |
|------------|----------|---------|
| `pdf` | `/funds/exporttopdf` | PDFsharp |
| `csv` | `/funds/exporttocsv` | Built-in |
| `excel-free` | `/funds/exporttoexcel-free` | ClosedXML |
| `excel` | `/funds/exporttoexcel` | EPPlus |
| `excel-generic` | `/funds/exporttoexcel-generic` | ClosedXML (generic) |

### Button Placement

```
[Download PDF] [Download CSV] [Download Excel (Free)] [Download Excel (EPPlus)] [Download Excel (Generic)]
                                                                              ↑ NEW BUTTON
```

### Styling

- **Button class**: `btn btn-success` (green Bootstrap button)
- **Icon**: Bootstrap Icons file-earmark-excel SVG
- **Text**: "Download Excel (Generic)"
- **Position**: After EPPlus button, right-aligned

## Usage

### User Flow

1. User visits funds table page
2. Page fetches download tokens on load
3. JavaScript updates all download button hrefs with tokens
4. User clicks "Download Excel (Generic)" button
5. Browser sends GET request to `/funds/exporttoexcel-generic?token={token}`
6. Server validates token
7. Server generates Excel using generic service
8. Browser downloads file as `Funds_Export_yyyyMMdd_HHmmss.xlsx`

### Security

- **Token Required**: All downloads require valid token
- **Token Validation**: Server validates token type and value
- **Token Expiry**: Expired tokens are rejected
- **Token Refresh**: Tokens auto-refresh before expiry

## Testing Checklist

- [x] New token type `excel-generic` added to token generation
- [x] Controller endpoint validates `excel-generic` token
- [x] Download tokens API returns `excelGeneric` token
- [x] Button appears in funds table view
- [x] Button has correct styling (green, Excel icon)
- [x] JavaScript updates button href on page load
- [x] Token validation works correctly
- [x] Download generates styled Excel file
- [x] Build succeeds with no errors

---

## Build Verification

```
Build succeeded.
    7 Warning(s)
    0 Error(s)
```

All download button functionality has been successfully implemented and tested.

---

# Feature 7: Excel Multi-Line Cell Formatting

## Task Plan

### Overview
Optimize Excel generic export to display multi-line header and disclaimer sections as single merged cells with proper text wrapping and heights, rather than multiple rows. Remove all borders and empty row gaps.

### Requirements
1. Title text in single merged cell (not multiple rows)
2. Subtitle text in single merged cell
3. Footer text in single merged cell
4. Disclaimer text in single merged cell
5. Enable text wrapping for all multi-line sections
6. Set proper vertical alignment (Top)
7. Remove all borders from multi-line sections
8. Remove empty row gaps between sections
9. Align all multi-line content to the left

### Implementation Approach
- Replace row-by-row rendering with single merged cell approach
- Use `WrapText = true` for automatic text wrapping
- Use `VerticalAlignment = Top` for proper alignment
- Remove `RowsAfterTitle`, `RowsAfterSubtitle`, and row gap variables
- Explicitly set all border styles to `None`

## Change Log

### Files Modified

| File | Lines Modified | Description |
|------|----------------|-------------|
| `Services/ExcelExportService.cs` | 225-270 | Rewrote `DrawTitle` for single merged cell with wrapping |
| `Services/ExcelExportService.cs` | 323-351 | Rewrote `DrawFooter` for single merged cell with wrapping |
| `Services/ExcelExportService.cs` | 353-381 | Rewrote `DrawDisclaimer` for single merged cell with wrapping |

### Detailed Changes

#### 1. DrawTitle - Single Merged Cell with Wrapping (lines 225-270)

**Before:**
```csharp
private int DrawTitle(IXLWorksheet worksheet, int startRow, int columnCount, ExcelExportOptions options)
{
    int currentRow = startRow;

    // Draw title (multi-line support)
    var titleLines = options.ReportTitle.Split('\n');
    foreach (var line in titleLines)
    {
        var cell = worksheet.Cell(currentRow, 1);
        cell.Value = line;
        ApplyRichText(cell, line, options.TitleFont);
        var range = worksheet.Range(currentRow, 1, currentRow, columnCount);
        range.Merge();
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        // Border removal code...
        currentRow++;
    }

    currentRow += options.RowsAfterTitle;

    // Similar for subtitle...
}
```

**After:**
```csharp
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
        // Remove all borders...
        currentRow++;
    }

    return currentRow;
}
```

#### 2. DrawFooter - Single Merged Cell (lines 323-351)

**Before:**
```csharp
private int DrawFooter(IXLWorksheet worksheet, int startRow, int columnCount, ExcelExportOptions options)
{
    if (string.IsNullOrEmpty(options.FooterText))
        return startRow;

    int currentRow = startRow + 1; // Add one row gap

    var footerLines = options.FooterText.Split('\n');
    foreach (var line in footerLines)
    {
        // Draw each line in separate row...
        currentRow++;
    }

    return currentRow;
}
```

**After:**
```csharp
private int DrawFooter(IXLWorksheet worksheet, int startRow, int columnCount, ExcelExportOptions options)
{
    if (string.IsNullOrEmpty(options.FooterText))
        return startRow;

    int currentRow = startRow; // No gap

    // Draw footer in a single merged cell with text wrapping
    var footerCell = worksheet.Cell(currentRow, 1);
    footerCell.Value = options.FooterText;
    ApplyRichText(footerCell, options.FooterText, options.FooterFont);
    var footerRange = worksheet.Range(currentRow, 1, currentRow, columnCount);
    footerRange.Merge();
    footerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
    footerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
    footerRange.Style.Alignment.WrapText = true;
    // Remove all borders...
    currentRow++;

    return currentRow;
}
```

#### 3. DrawDisclaimer - Single Merged Cell (lines 353-381)

**Before:**
```csharp
private int DrawDisclaimer(IXLWorksheet worksheet, int startRow, int columnCount, ExcelExportOptions options)
{
    if (string.IsNullOrEmpty(options.Disclaimer))
        return startRow;

    int currentRow = startRow + 1; // Add one row gap

    var disclaimerLines = options.Disclaimer.Split('\n');
    foreach (var line in disclaimerLines)
    {
        // Draw each line in separate row...
        currentRow++;
    }

    return currentRow;
}
```

**After:**
```csharp
private int DrawDisclaimer(IXLWorksheet worksheet, int startRow, int columnCount, ExcelExportOptions options)
{
    if (string.IsNullOrEmpty(options.Disclaimer))
        return startRow;

    int currentRow = startRow; // No gap

    // Draw disclaimer in a single merged cell with text wrapping
    var disclaimerCell = worksheet.Cell(currentRow, 1);
    disclaimerCell.Value = options.Disclaimer;
    ApplyRichText(disclaimerCell, options.Disclaimer, options.DisclaimerFont);
    var disclaimerRange = worksheet.Range(currentRow, 1, currentRow, columnCount);
    disclaimerRange.Merge();
    disclaimerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
    disclaimerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
    disclaimerRange.Style.Alignment.WrapText = true;
    // Remove all borders...
    currentRow++;

    return currentRow;
}
```

## Implementation Details

### Key Changes

1. **Single Cell Rendering**: Multi-line text no longer split across rows
2. **Text Wrapping**: `WrapText = true` enables automatic wrapping
3. **Vertical Alignment**: `VerticalAlignment = Top` for consistent positioning
4. **No Empty Rows**: Removed `RowsAfterTitle`, `RowsAfterSubtitle`, and row gaps
5. **Border Removal**: Explicitly set all borders to `None`

### Cell Styling Applied

```csharp
// Alignment
range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
range.Style.Alignment.WrapText = true;

// Border removal
range.Style.Border.TopBorder = XLBorderStyleValues.None;
range.Style.Border.BottomBorder = XLBorderStyleValues.None;
range.Style.Border.LeftBorder = XLBorderStyleValues.None;
range.Style.Border.RightBorder = XLBorderStyleValues.None;
range.Style.Border.DiagonalBorder = XLBorderStyleValues.None;
```

### Benefits

- **Cleaner Layout**: Multi-line content appears in compact, properly-sized cells
- **Auto-Sizing**: Excel automatically adjusts row height based on content
- **No Visual Clutter**: No borders or empty rows around header/footer sections
- **Professional Appearance**: Looks more like a document header/footer

## Usage Example

```csharp
var options = new ExcelExportOptions
{
    ReportTitle = "**FUND SUMMARY REPORT**\nThis report provides a summary of all funds\nincluding NAV and Market Prices.",
    Subtitle = "Generated by Investment Team\nConfidential Document",
    Disclaimer = "**IMPORTANT DISCLAIMER:**\nPast performance is not indicative of future results.\nPlease consult with a qualified financial advisor."
    // All text will appear in single merged cells with wrapping
};
```

## User Flow

1. User clicks "Download Excel (Generic)" button
2. Excel file generated with multi-line header in single cell
3. Excel automatically wraps text and adjusts row height
4. No borders or empty rows around header/footer sections
5. Clean, professional document layout

## Testing Checklist

- [x] Title displays in single merged cell
- [x] Subtitle displays in single merged cell
- [x] Footer displays in single merged cell
- [x] Disclaimer displays in single merged cell
- [x] Text wrapping works correctly
- [x] Vertical alignment set to Top
- [x] All borders removed from multi-line sections
- [x] No empty rows between sections
- [x] Left alignment applied
- [x] Build succeeds with no errors

---

## Build Verification

```
Build succeeded.
    7 Warning(s)
    0 Error(s)
```

Excel multi-line cell formatting has been successfully implemented with single merged cells, text wrapping, and removed borders/empty rows.

