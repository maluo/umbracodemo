# Umbraco13 - Fund Management Application

A web application built with Umbraco CMS v13 for managing and displaying investment fund information, including historical NAV values, market prices, and performance metrics.

## Features

- **Fund Management**: View and manage investment fund details
- **Historical NAV Data**: Track Net Asset Value history with interactive charts
- **Data Visualization**: SVG bar charts aligned with tabular data
- **Export Functionality**:
  - PDF export with customizable formatting, metadata, and height options
  - Excel export with auto-sized columns, rich text formatting, and border options
- **Download Security**: Token-based download authentication with auto-refresh
- **Performance**: In-memory caching of funds.json data using MediaFileManager
- **Responsive Design**: Mobile-friendly interface
- **Multi-line Headers**: Support for complex report headers
- **Bold Text Markup**: Use `**text**` for bold formatting in exports
- **ViewComponents**: Modular, reusable components for funds, tables, and navigation history

## Tech Stack

- **Framework**: Umbraco CMS 13.10.0
- **Backend**: ASP.NET Core 8.0
- **Frontend**: Razor Views, JavaScript, Highcharts 5.0.7
- **PDF Library**: PdfSharp
- **Excel Library**: ClosedXML
- **Media Services**: Umbraco MediaFileManager & IMediaService

## Installation

1. Clone the repository
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
3. Build the project:
   ```bash
   dotnet build
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

## Project Structure

```
Umbraco13/
├── Controllers/              # API controllers
├── Models/                   # View models and data models
├── Services/                 # Business logic services
│   ├── PdfExportService.cs
│   ├── ExcelExportService.cs
│   ├── FundsJsonService.cs           # In-memory funds.json caching
│   ├── NavHistoryService.cs          # Navigation history management
│   ├── DownloadTokenService.cs       # Token-based download security
│   ├── FundService.cs
│   └── FundHistoricalNavService.cs
├── ViewComponents/           # Reusable view components
│   ├── FundsTableViewComponent.cs
│   ├── FundsJsonViewComponent.cs
│   ├── HistoricalNavTableViewComponent.cs
│   └── NavHistoryViewComponent.cs
├── Helpers/                  # Helper utilities
│   └── FundTableConverter.cs
├── Views/                    # Razor views
│   └── Shared/
│       └── Components/
│       └── HistoricalNavTable/
│           └── Default.highcharts.cshtml  # Highcharts chart with alignment
├── wwwroot/                  # Static assets
│   └── js/
│       └── scripts.js
└── appsettings.json          # Configuration
```

## Usage

### PDF Export Service

```csharp
var options = new PdfExportOptions
{
    ReportTitle = "FUND SUMMARY REPORT",
    ItemsPerPage = 25,
    Disclaimer = "This report contains historical NAV prices and is for informational purposes only."
};

var pdfBytes = _pdfExportService.ExportToPdf(data, columns, options);
```

### Excel Export Service

```csharp
var options = new ExcelExportOptions
{
    ReportTitle = "Fund Data Export",
    Disclaimer = "**Confidential** - For internal use only.",
    AutoSizeColumns = true,
    EnableRichTextFormatting = true
};

var excelBytes = _excelExportService.ExportToExcel(data, columns, options);
```

### Funds JSON Service (In-Memory Caching)

```csharp
// Automatically loads funds.json at startup and caches in memory
var fundsData = _fundsJsonService.GetFundsData();

// Retrieve NAV history for a specific fund
var navHistory = _fundsJsonService.GetNavHistory("TICKER123");
```

### Navigation History Service

```csharp
// Get historical NAV data for a fund ticker
var history = await _navHistoryService.GetNavHistoryAsync("TICKER123");

foreach (var entry in history)
{
    Console.WriteLine($"{entry.Date}: NAV={entry.NavPrice}, Market={entry.MarketPrice}");
}
```

### Download Token Service

```csharp
// Generate a secure token for PDF download (valid for 30 minutes)
var pdfToken = _downloadTokenService.GenerateDownloadToken("pdf");

// Validate token before allowing download
var isValid = _downloadTokenService.ValidateDownloadToken(token, "pdf");
if (isValid)
{
    // Proceed with download
}
```

### ViewComponents

```csharp
// Invoke in Razor views
@await Component.InvokeAsync("FundsTable", new { tickerCode = "TICKER123" })
@await Component.InvokeAsync("NavHistory", new { tickerCode = "TICKER123" })
@await Component.InvokeAsync("FundsJson")
```

### Media Library Service (MediaFileManager)

```csharp
/// FundsJsonService uses MediaFileManager to load funds.json from Umbraco media library
/// No direct instantiation needed - registered as singleton in DI container

public class FundsJsonService : IFundsJsonService
{
    private readonly MediaFileManager _mediaFileManager;
    private readonly IMediaService _mediaService;

    // Automatically loads and caches funds.json at startup
    // Uses MediaFileManager.FileSystem to access media files
    // Uses IMediaService to locate funds.json in media library
}
```

### Highcharts Alignment JavaScript

```javascript
/**
 * Render Highcharts bar chart with dynamic table column alignment
 * Automatically calculates bar widths and positions to match table columns
 *
 * Features:
 * - Dynamic width measurement from table cells
 * - Responsive padding (mobile vs desktop)
 * - Left margin to align with table data columns
 * - Auto-resize on window resize
 */
function renderHighchartsBarChart(data, containerId, tableColumnWidth, labelColumnWidth) {
    const isMobile = window.innerWidth < 768;

    // Responsive padding: more space on mobile
    const groupPadding = isMobile ? 0.30 : 0.20;
    const pointPadding = isMobile ? 0.05 : 0.02;

    // Calculate bar width as percentage of column
    const barWidth = isMobile
        ? Math.floor(tableColumnWidth * 0.25)
        : Math.floor(tableColumnWidth * 0.30);

    Highcharts.chart(containerId, {
        chart: {
            type: 'column',
            marginLeft: labelColumnWidth,  // Skip label column
            backgroundColor: '#f8f9fa',
            borderRadius: 8
        },
        plotOptions: {
            column: {
                pointWidth: barWidth,      // Dynamic width
                groupPadding: groupPadding, // Space between date groups
                pointPadding: pointPadding, // Space between NAV/Market bars
                borderRadius: 3
            }
        },
        series: [
            { name: 'NAV Price', data: navPrices, color: '#3b82f6' },
            { name: 'Market Price', data: marketPrices, color: '#22c55e' }
        ]
    });
}

// Call with measured table dimensions
function renderChart(paginatedNavs) {
    setTimeout(() => {
        const table = document.getElementById('historical-nav-table');
        const labelCell = table.querySelector('tbody tr td:first-child');
        const dataCell = table.querySelector('tbody tr td:nth-child(2)');

        renderHighchartsBarChart(
            paginatedNavs,
            'nav-chart',
            dataCell?.offsetWidth || 120,
            labelCell?.offsetWidth || 0
        );
    }, 50); // Wait for DOM render
}
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## MIT License

Copyright (c) 2025

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

---

## What You Can Do (MIT License)

✅ **Commercial Use**: You can use this software in commercial applications
✅ **Modification**: You can modify the source code to fit your needs
✅ **Distribution**: You can distribute copies of the software
✅ **Sublicense**: You can grant others the right to use this software
✅ **Private Use**: You can use this software privately

## Requirements (MIT License)

- Include the copyright notice and license text in your distributions
- Software is provided "as is" without warranty

## Documentation

For detailed implementation documentation, see the `notes/` folder:

### Core Services
- [Funds JSON Service](notes/refactoring/Refactor-FundsJsonService-to-use-MediaFileManager_2025-02-15.md) - In-memory caching with MediaFileManager
- [Navigation History Service](notes/features/navigation/NavHistory-Integration-2026-02-14.md) - NAV data management
- [Download Token Service](notes/configuration/auth.md) - Secure token-based downloads
- [Media Library Integration](notes/helpers/file_service.md) - MediaFileManager usage

### Export Features
- [PDF Export Service Guide](notes/pdf-features/pdf_export_service_usage.md)
- [PDF Metadata Feature](notes/pdf-features/PDFMetadataFeature-2026-02-08.md)
- [PDF Height Options](notes/pdf-features/PDFExportLastRowAndHeightOptions-2026-02-06.md)
- [Excel Rich Text Formatting](notes/excel-export/ExcelPartialRichTextFormatting-2026-02-07.md)
- [Excel Export Height Options](notes/excel-export/ExcelExportHeightOptions-2026-02-06.md)

### Features & Components
- [Historical NAV Table](notes/features/navigation/HistoricalNavPivotedTable_2026-01-31.md) - Pivoted table layout
- [Dynamic Table Sorting & Pagination](notes/features/tables/dynamic_table_sorting_pagination.md)
- [Highcharts Integration](notes/features/charts/Highcharts507NavChart-2026-02-08.md) - Interactive charts
- [Highcharts Alignment](notes/features/charts/HighchartsNavChartRefinement-2026-02-08.md) - Bar-to-column alignment
- [Border Options for Sections](notes/features/tables/BorderOptionsForSections-2026-02-06.md)

### Configuration
- [Git Configuration](notes/configuration/Update%20Git%20Configuration%202026-02-05.md)
- [Umbraco 13 Git Conflict Resolution](notes/configuration/Git_Conflict_Resolution_Umbraco13_2026-02-05.md)

## Support

For issues, questions, or contributions, please refer to the project repository.

---

**Free to use for personal and commercial purposes.**
