# Multiline Disclaimer Rich Text Feature - 2026-02-07

## Task Name
Add multiline disclaimer support with per-line rich text formatting in Excel export service

## Task Checklist
- [x] Modify `DrawDisclaimer` method in Umbraco13 ExcelExportService to support multiline with per-line formatting
- [x] Modify `DrawDisclaimer` method in API ExcelExportService to support multiline with per-line formatting
- [x] Update XML comments for `ApplyRichText` method to reflect new disclaimer behavior
- [x] Log task completion

## Implementation Details

### Technical Approach
The previous implementation drew the disclaimer as a single merged cell, which prevented per-line rich text formatting. ClosedXML doesn't support partial cell formatting within a single cell.

**Solution**: Split the disclaimer by `\n` and draw each line as a separate row, allowing individual `ApplyRichText` calls with `allowBoldFromMarkers: true` for each line.

### Key Changes

1. **`DrawDisclaimer` method** (both Umbraco13 and API versions):
   - Split `options.Disclaimer` by `\n` using `StringSplitOptions.None`
   - Loop through lines and draw each in its own merged row
   - Apply `ApplyRichText` with `allowBoldFromMarkers: true` for each line to support `**bold**` markers
   - Handle borders for each row independently
   - Distribute custom height evenly across all lines if `DisclaimerHeightPixels > 0`

2. **Height Calculation**:
   ```csharp
   if (options.DisclaimerHeightPixels > 0)
   {
       double heightPerLine = options.DisclaimerHeightPixels * 0.75 / disclaimerLines.Length;
       worksheet.Row(currentRow).Height = heightPerLine;
   }
   ```

3. **Updated XML Documentation**:
   - Clarified that disclaimer now supports `**bold**` markers per line
   - Footer still respects `FooterFont.Bold` setting (markers stripped only)
   - Title and subtitle continue to support `**bold**` markers

### Files Modified

1. **`/Umbraco13/Services/ExcelExportService.cs`**
   - Updated `DrawDisclaimer` method (lines 530-578)
   - Updated `ApplyRichText` XML comment (lines 580-591)

2. **`/API/FundsApi/Services/ExcelExportService.cs`**
   - Updated `DrawDisclaimer` method (lines 313-361)
   - Updated `ApplyRichText` XML comment (lines 363-374)

## Change Log

### What Was Changed
- Excel disclaimer now splits by `\n` and renders each line as a separate row
- Each disclaimer line can have independent rich text formatting (some lines bold, some not)
- `**bold**` markers in disclaimer lines now work correctly (entire line is bolded)
- Custom disclaimer height is distributed evenly across all lines when specified

### Usage Example

```csharp
var excelOptions = new ExcelExportOptions
{
    ReportTitle = "My Report",
    Disclaimer = "This is a normal line.\n**This is a bold line.**\nThis is another normal line.\n**Another bold line.**",
    DisclaimerHeightPixels = 60,  // Will distribute 45 points (60 * 0.75) across 4 lines = ~11.25 points per line
    DisclaimerFont = new ExcelFontStyle { FontSize = 8, FontColor = "#000000" }
};
```

### Notes
- Each line in the disclaimer is rendered as a separate merged cell row
- Borders are applied to each row independently (when `ShowDisclaimerBorders = true`)
- The solution works around ClosedXML's limitation of not supporting partial cell formatting
- Backward compatible - single-line disclaimers work exactly as before
