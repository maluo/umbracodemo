# Excel Export Height Options Feature - 2026-02-06

## Task Name
Add pixel-based height options for heading and disclaimer sections in Excel print server

## Implementation Plan

1. Add `HeadingHeightPixels` property to `ExcelExportOptions` class
2. Add `DisclaimerHeightPixels` property to `ExcelExportOptions` class
3. Update `DrawTitle` method to apply custom heading height
4. Update `DrawDisclaimer` method to apply custom disclaimer height
5. Apply changes to both Umbraco13 and API service files

## Change Log

### Files Modified

1. **ExcelExportModels.cs** (API/FundsApi/Models/ExportModels/)
   - Added `HeadingHeightPixels` property (default: 0 = auto-height)
   - Added `DisclaimerHeightPixels` property (default: 0 = auto-height)

2. **ExcelExportService.cs** (Umbraco13/Services/)
   - Added `HeadingHeightPixels` and `DisclaimerHeightPixels` properties
   - Modified `DrawTitle` method to apply custom heading height when specified
   - Modified `DrawDisclaimer` method to apply custom disclaimer height when specified
   - Height conversion: pixels to points (1 pixel = 0.75 points in Excel)

3. **ExcelExportService.cs** (API/FundsApi/Services/)
   - Modified `DrawTitle` method to apply custom heading height when specified
   - Modified `DrawDisclaimer` method to apply custom disclaimer height when specified

### Implementation Details

- Row height is set using `worksheet.Row(currentRow).Height = value`
- Conversion formula: `pixels * 0.75` converts to Excel row height points
- When set to 0, auto-height behavior is preserved (existing default)
- Both title and subtitle rows use the same heading height setting

### Usage Example

```csharp
var options = new ExcelExportOptions
{
    ReportTitle = "My Report",
    Disclaimer = "Legal disclaimer text...",
    HeadingHeightPixels = 50,     // Custom heading height
    DisclaimerHeightPixels = 80    // Custom disclaimer height
};
```

## Notes

- Changes are backward compatible (default value 0 maintains existing auto-height behavior)
- Applied consistently across both service implementations
