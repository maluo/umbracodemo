# Border Options for Heading, Disclaimer, and Footer - 2026-02-06

## Task Name
Add border options for heading, disclaimer, and footer sections in PDF and Excel export services

## Implementation Plan

1. Add `ShowHeadingBorders`, `ShowDisclaimerBorders`, and `ShowFooterBorders` properties to export options
2. Update Excel service methods to conditionally apply borders
3. Update PDF service methods to conditionally apply borders
4. Apply changes to both Umbraco13 and API service files

## Change Log

### Files Modified

1. **ExcelExportModels.cs** (API/FundsApi/Models/ExportModels/)
   - Added `ShowHeadingBorders` property (default: false)
   - Added `ShowDisclaimerBorders` property (default: false)
   - Added `ShowFooterBorders` property (default: false)

2. **ExcelExportModels.cs** (Umbraco13/Services/)
   - Added `ShowHeadingBorders`, `ShowDisclaimerBorders`, `ShowFooterBorders` properties

3. **ExcelExportService.cs** (Umbraco13/Services/)
   - Updated `DrawTitle` to apply borders to title and subtitle when `ShowHeadingBorders = true`
   - Updated `DrawDisclaimer` to apply borders when `ShowDisclaimerBorders = true`
   - Updated `DrawFooter` to apply borders when `ShowFooterBorders = true`

4. **ExcelExportService.cs** (API/FundsApi/Services/)
   - Updated `DrawTitle` to apply borders when `ShowHeadingBorders = true`
   - Updated `DrawDisclaimer` to apply borders when `ShowDisclaimerBorders = true`
   - Updated `DrawFooter` to apply borders when `ShowFooterBorders = true`

5. **PdfExportModels.cs** (API/FundsApi/Models/ExportModels/)
   - Added `ShowHeadingBorders`, `ShowDisclaimerBorders`, `ShowFooterBorders` properties

6. **PdfExportModels.cs** (Umbraco13/Services/)
   - Added `ShowHeadingBorders`, `ShowDisclaimerBorders`, `ShowFooterBorders` properties

7. **PdfExportService.cs** (Umbraco13/Services/)
   - Updated `DrawPageHeader` to draw rectangle borders around heading when `ShowHeadingBorders = true`
   - Updated `DrawDisclaimer` to draw rectangle borders when `ShowDisclaimerBorders = true`
   - Updated `DrawReportFooter` to draw rectangle borders when `ShowFooterBorders = true`

8. **PdfExportService.cs** (API/FundsApi/Services/)
   - Updated `DrawPageHeader` to draw rectangle borders when `ShowHeadingBorders = true`
   - Updated `DrawDisclaimer` to draw rectangle borders when `ShowDisclaimerBorders = true`
   - Updated `DrawReportFooter` to draw rectangle borders when `ShowFooterBorders = true`

### Implementation Details

**Excel Border Style:**
- Border style: `XLBorderStyleValues.Thin`
- Applied to merged cell ranges
- Borders removed when options are set to false

**PDF Border Style:**
- Pen: `XPen(XColors.Gray, 0.5)`
- Rectangle drawn around the entire section
- Width calculated based on margins and page width

### Usage Example

```csharp
// Excel
var excelOptions = new ExcelExportOptions
{
    ReportTitle = "My Report",
    Disclaimer = "Legal disclaimer...",
    ShowHeadingBorders = true,      // Add borders around title/subtitle
    ShowDisclaimerBorders = true,   // Add borders around disclaimer
    ShowFooterBorders = true        // Add borders around footer
};

// PDF
var pdfOptions = new PdfExportOptions
{
    ReportTitle = "My Report",
    Disclaimer = "Legal disclaimer...",
    ShowHeadingBorders = true,      // Add borders around title/subtitle
    ShowDisclaimerBorders = true,   // Add borders around disclaimer
    ShowFooterBorders = true        // Add borders around footer
};
```

## Notes

- Changes are backward compatible (default value false maintains existing no-border behavior)
- Excel borders are applied to merged cell ranges using thin border style
- PDF borders are drawn as rectangles with 0.5 width gray pen
- Applied consistently across both service implementations
