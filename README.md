# Umbraco13 - Fund Management Application

A web application built with Umbraco CMS v13 for managing and displaying investment fund information, including historical NAV values, market prices, and performance metrics.

## Features

- **Fund Management**: View and manage investment fund details
- **Historical NAV Data**: Track Net Asset Value history with interactive charts
- **Data Visualization**: SVG bar charts aligned with tabular data
- **Export Functionality**:
  - PDF export with customizable formatting
  - Excel export with auto-sized columns
- **Responsive Design**: Mobile-friendly interface
- **Multi-line Headers**: Support for complex report headers
- **Bold Text Markup**: Use `**text**` for bold formatting in exports

## Tech Stack

- **Framework**: Umbraco CMS 13.10.0
- **Backend**: ASP.NET Core 8.0
- **Frontend**: Razor Views, JavaScript, SVG
- **PDF Library**: PdfSharp
- **Excel Library**: ClosedXML

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
├── Controllers/          # API controllers
├── Models/              # View models and data models
├── Services/            # Business logic services
│   ├── PdfExportService.cs
│   ├── ExcelExportService.cs
│   └── ...
├── Views/               # Razor views
│   └── Shared/
│       └── Components/
├── wwwroot/             # Static assets
└── appsettings.json     # Configuration
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
    Disclaimer = "**Confidential** - For internal use only."
};

var excelBytes = _excelExportService.ExportToExcel(data, columns, options);
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

For detailed usage documentation, see:
- [PDF Export Service Guide](pdf_export_service_usage.md)
- [Historical NAV Table Feature](HistoricalNavPivotedTable_2026-01-31.md)

## Support

For issues, questions, or contributions, please refer to the project repository.

---

**Free to use for personal and commercial purposes.**
