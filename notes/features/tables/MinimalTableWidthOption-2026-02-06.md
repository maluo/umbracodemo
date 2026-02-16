# Minimal Table Width Option for PDF and Excel Export - 2026-02-06

## Task Name
Add minimal table width option with pixel units for PDF and Excel export services

## Implementation Plan

1. Add `TableMinimalWidthPixels` property to `ExcelExportOptions` class
2. Add `TableMinimalWidthPixels` property to `PdfExportOptions` class
3. Implement `EnforceMinimalTableWidth` method for Excel service
4. Update `CalculateColumnWidths` method in PDF service to enforce minimal width
5. Apply changes to both Umbraco13 and API service files

## Change Log

### Files Modified

1. **ExcelExportModels.cs** (API/FundsApi/Models/ExportModels/)
   - Added `TableMinimalWidthPixels` property (default: 0 = auto-width)

2. **ExcelExportModels.cs** (Umbraco13/Services/)
   - Added `TableMinimalWidthPixels` property

3. **ExcelExportService.cs** (Umbraco13/Services/)
   - Added `EnforceMinimalTableWidth` method to enforce minimal width after auto-fit
   - Updated `ExportToExcel` to call `EnforceMinimalTableWidth` when `TableMinimalWidthPixels > 0`

4. **ExcelExportService.cs** (API/FundsApi/Services/)
   - Added `EnforceMinimalTableWidth` method
   - Updated `ExportToExcel` to call `EnforceMinimalTableWidth` when `TableMinimalWidthPixels > 0`

5. **PdfExportModels.cs** (API/FundsApi/Models/ExportModels/)
   - Added `TableMinimalWidthPixels` property

6. **PdfExportModels.cs** (Umbraco13/Services/)
   - Added `TableMinimalWidthPixels` property

7. **PdfExportService.cs** (Umbraco13/Services/)
   - Updated `CalculateColumnWidths` to enforce minimal table width at the end of the method

8. **PdfExportService.cs** (API/FundsApi/Services/)
   - Updated `CalculateColumnWidths` to enforce minimal table width at the end of the method

### Implementation Details

**Excel Service:**
- Conversion: 1 character ≈ 7 pixels (based on default Calibri 11pt font)
- After auto-fit, calculates current total width in pixels
- If current width < minimal width, distributes difference equally across all columns
- Formula: `newWidth = (currentWidthInPixels + widthToAddPerColumn) / pixelsPerCharacter`

**PDF Service:**
- Conversion: 1 pixel = 0.75 points (PDF units)
- After calculating column widths, checks if total width meets minimum
- If current width < minimal width, distributes difference equally across all columns
- Formula: `minimalWidthPoints = pixels * 0.75`

### Usage Example

```csharp
// Excel
var excelOptions = new ExcelExportOptions
{
    TableMinimalWidthPixels = 800  // Ensure table is at least 800px wide
};

// PDF
var pdfOptions = new PdfExportOptions
{
    TableMinimalWidthPixels = 800  // Ensure table is at least 800px wide
};
```

## Notes

- Changes are backward compatible (default value 0 maintains existing auto-width behavior)
- Width is distributed equally across all columns when minimal width is enforced
- For Excel, the enforcement happens after auto-fit to ensure content-based sizing first
- For PDF, the enforcement happens at the end of column width calculation
- Applied consistently across both service implementations
