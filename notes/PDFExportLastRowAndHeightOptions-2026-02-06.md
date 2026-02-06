# PDF Export Enhancements - Last Row and Height Options - 2026-02-06

## Task Name
Add last row definition and heading/disclaimer height options to PDF print server

## Implementation Plan

1. Add `PdfLastRowDefinition` class for custom last row with user-provided values
2. Add `PdfFontStyle` class for font styling options
3. Add `LastRow`, `HeadingHeightPixels`, and `DisclaimerHeightPixels` properties to `PdfExportOptions`
4. Update `ExportToPdf` method to support custom last row
5. Create `DrawLastRow` method to render the custom last row
6. Update `DrawPageHeader` method to apply custom heading height
7. Update `DrawDisclaimer` method to apply custom disclaimer height
8. Apply changes to both Umbraco13 and API service files

## Change Log

### Files Modified

1. **PdfExportModels.cs** (API/FundsApi/Models/ExportModels/)
   - Added `PdfFontStyle` class for font styling (FontFamily, FontSize, Bold, Italic)
   - Added `PdfLastRowDefinition` class (CellValues list, optional FontStyle)
   - Added `LastRow` property to `PdfExportOptions`
   - Added `HeadingHeightPixels` property (default: 0 = auto-height)
   - Added `DisclaimerHeightPixels` property (default: 0 = auto-height)

2. **PdfExportModels.cs** (Umbraco13/Services/)
   - Added `PdfFontStyle` class
   - Added `PdfLastRowDefinition` class
   - Added `LastRow`, `HeadingHeightPixels`, `DisclaimerHeightPixels` properties

3. **PdfExportService.cs** (Umbraco13/Services/)
   - Modified `ExportToPdf` to call `DrawLastRow` when `LastRow` is provided
   - Updated `DrawPageHeader` to apply custom heading height when specified
   - Updated `DrawDisclaimer` to apply custom disclaimer height when specified
   - Added `DrawLastRow` method with grey background and custom font style support

4. **PdfExportService.cs** (API/FundsApi/Services/)
   - Modified `ExportToPdf` to call `DrawLastRow` when `LastRow` is provided
   - Updated `DrawPageHeader` to apply custom heading height when specified
   - Updated `DrawDisclaimer` to apply custom disclaimer height when specified
   - Added `DrawLastRow` method with grey background and custom font style support

### Implementation Details

- **Last Row Feature**: Custom last row appears after the average row (if shown) and before the disclaimer
- **Font Styling**: Last row can have custom font style (family, size, bold, italic) or defaults to bold
- **Height Conversion**: Pixels to PDF points conversion: `pixels * 0.75`
- **Line Heights**:
  - Title line height: custom or default 20 points
  - Subtitle line height: custom or default 15 points
  - Disclaimer line height: custom or default 12 points
- **Background**: Last row has grey background (#F0F0F0) like the average row

### Usage Example

```csharp
var options = new PdfExportOptions
{
    ReportTitle = "My Report",
    Disclaimer = "Legal disclaimer text...",

    // Custom last row
    LastRow = new PdfLastRowDefinition
    {
        CellValues = new List<string> { "Total", "$1,234,567", "100%" },
        FontStyle = new PdfFontStyle { FontSize = 11, Bold = true }
    },

    // Custom heights in pixels
    HeadingHeightPixels = 25,     // Title and subtitle lines
    DisclaimerHeightPixels = 15   // Disclaimer lines
};
```

## Notes

- Changes are backward compatible (default value 0 maintains existing auto-height behavior)
- Last row only renders if `CellValues` list has items
- Font style for last row defaults to bold if not specified
- Applied consistently across both service implementations
