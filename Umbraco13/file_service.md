# File Export & Download Service - Complete Documentation

## Overview

The application provides multiple export formats for fund data with automatic token-based authentication:

- **CSV** - Using StringBuilder (no external library)
- **PDF** - Using PdfSharp (free, no license)
- **Excel** - Two options:
  - ClosedXML (free, no license)
  - EPPlus (requires license configuration)

All exports include:
- Header section with title and timestamp
- Data table with fund information
- Footer section with summary and disclaimer

---

## 1. Export Endpoints

### Location: `Controllers/FundsController.cs`

#### CSV Export
```
GET /funds/exporttocsv?token=...
```
- **Attribute**: `[ValidateDownloadToken("csv")]`
- **Returns**: `text/csv` file
- **Filename**: `Funds_Export_{timestamp}.csv`
- **Implementation**: StringBuilder with manual CSV escaping

#### PDF Export
```
GET /funds/exporttopdf?token=...
```
- **Attribute**: `[ValidateDownloadToken("pdf")]`
- **Returns**: `application/pdf` file
- **Filename**: `Funds_Export_{timestamp}.pdf`
- **Implementation**: PdfSharp with manual table layout

#### Excel Export (ClosedXML - Free)
```
GET /funds/exporttoexcel-free?token=...
```
- **Attribute**: `[ValidateDownloadToken("excel-free")]`
- **Returns**: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- **Filename**: `Funds_Export_{timestamp}.xlsx`
- **Implementation**: ClosedXML library

#### Excel Export (EPPlus - Licensed)
```
GET /funds/exporttoexcel?token=...
```
- **Attribute**: `[ValidateDownloadToken("excel")]`
- **Returns**: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- **Filename**: `Funds_Export_{timestamp}.xlsx`
- **Implementation**: EPPlus library (requires NonCommercial license in appsettings)

---

## 2. CSV Export Implementation

### Using StringBuilder (No External Library)

```csharp
[HttpGet("exporttocsv")]
[ValidateDownloadToken("csv")]
public async Task<IActionResult> ExportToCsv()
{
    var funds = await _fundService.GetAllFundsAsync();
    var csvBuilder = new StringBuilder();

    // Header Section
    csvBuilder.AppendLine("FUND SUMMARY REPORT");
    csvBuilder.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    csvBuilder.AppendLine();

    // Table Header
    csvBuilder.AppendLine("Fund Name,Ticker Code,NAV Price,Market Price,Hold In Trust");

    // Data Rows
    foreach (var fund in funds)
    {
        string fundName = EscapeCsvValue(fund.FundName);
        string tickerCode = EscapeCsvValue(fund.TickerCode);
        string navPrice = fund.NavPrice.ToString();
        string marketPrice = fund.MarketPrice.ToString();
        string holdInTrust = fund.HoldInTrust ?? "";
        csvBuilder.AppendLine($"{fundName},{tickerCode},{navPrice},{marketPrice},{holdInTrust}");
    }

    // Footer Section
    csvBuilder.AppendLine();
    csvBuilder.AppendLine("END OF REPORT");
    csvBuilder.AppendLine($"Total Funds: {funds.Count}");
    csvBuilder.AppendLine("This document is confidential and intended for internal use only.");

    var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
    return File(csvBytes, "text/csv", $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
}

// CSV value escaping helper
private string EscapeCsvValue(string value)
{
    if (string.IsNullOrEmpty(value)) return "";
    if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
    {
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
    return value;
}
```

**Features**:
- Proper CSV escaping for values containing commas, quotes, or newlines
- UTF-8 encoding
- Header and footer sections

---

## 3. PDF Export Implementation

### Using PdfSharp (Free, No License)

```csharp
[HttpGet("exporttopdf")]
[ValidateDownloadToken("pdf")]
public async Task<IActionResult> ExportToPdf()
{
    var funds = await _fundService.GetAllFundsAsync();

    // Create PDF document
    var document = new PdfDocument();
    var page = document.AddPage();
    page.Size = PdfSharp.PageSize.A4;

    var gfx = XGraphics.FromPdfPage(page);
    var font = new XFont("Arial", 10);
    var fontBold = new XFont("Arial", 10);
    var fontHeader = new XFont("Arial", 20);
    var fontFooter = new XFont("Arial", 8);

    // Define table dimensions
    double marginLeft = 50;
    double marginTop = 80;
    double rowHeight = 25;
    double colWidth1 = 150; // Fund Name
    double colWidth2 = 100; // Ticker Code
    double colWidth3 = 80;  // NAV Price
    double colWidth4 = 80;  // Market Price
    double colWidth5 = 100; // Hold In Trust

    // Header Section
    gfx.DrawString("FUND SUMMARY REPORT", fontHeader, XBrushes.Black,
        new XRect(0, 30, page.Width, 40), XStringFormats.TopCenter);
    gfx.DrawString($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", font, XBrushes.Black,
        new XRect(0, 60, page.Width, 20), XStringFormats.TopCenter);

    // Table Header Row
    double yPos = marginTop;
    XPen pen = new XPen(XColors.Gray, 0.5);
    XBrush brushHeader = new XSolidBrush(XColor.FromArgb(224, 224, 224));
    XBrush brushWhite = new XSolidBrush(XColors.White);

    // Draw header cells with borders
    gfx.DrawRectangle(pen, brushHeader, marginLeft, yPos, colWidth1, rowHeight);
    gfx.DrawString("Fund Name", fontBold, XBrushes.Black,
        new XRect(marginLeft, yPos, colWidth1, rowHeight), XStringFormats.Center);
    // ... (repeat for other columns)

    // Data Rows
    yPos += rowHeight;
    foreach (var fund in funds)
    {
        // Check for new page
        if (yPos + rowHeight > page.Height - 100)
        {
            page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            gfx = XGraphics.FromPdfPage(page);
            yPos = marginTop;
        }

        // Draw data cells
        gfx.DrawRectangle(pen, brushWhite, marginLeft, yPos, colWidth1, rowHeight);
        gfx.DrawString(fund.FundName, font, XBrushes.Black,
            new XRect(marginLeft + 5, yPos, colWidth1 - 10, rowHeight), XStringFormats.CenterLeft);
        // ... (repeat for other columns)
        yPos += rowHeight;
    }

    // Footer Section
    yPos = page.Height - 70;
    gfx.DrawString("END OF REPORT", fontBold, XBrushes.Black,
        new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
    gfx.DrawString($"Total Funds: {funds.Count}", font, XBrushes.Black,
        new XRect(0, yPos + 20, page.Width, 15), XStringFormats.TopCenter);
    gfx.DrawString("This document is confidential and intended for internal use only.",
        fontFooter, XBrushes.Black,
        new XRect(0, yPos + 35, page.Width, 15), XStringFormats.TopCenter);

    using var stream = new MemoryStream();
    document.Save(stream, false);
    var pdfBytes = stream.ToArray();

    return File(pdfBytes, "application/pdf", $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
}
```

**Features**:
- Grid-based table layout with borders
- Automatic pagination when data exceeds page height
- Centered header and footer
- Gray background for header row
- Cell borders for all data cells

---

## 4. Excel Export Implementation

### Option 1: ClosedXML (Free)

```csharp
[HttpGet("exporttoexcel-free")]
[ValidateDownloadToken("excel-free")]
public async Task<IActionResult> ExportToExcelFree()
{
    var funds = await _fundService.GetAllFundsAsync();

    using var workbook = new XLWorkbook();
    var worksheet = workbook.Worksheets.Add("Funds");

    // Header Section (rows 1-4)
    worksheet.Cell("A1").Value = "FUND SUMMARY REPORT";
    worksheet.Range("A1:E1").Merge();
    worksheet.Cell("A1").Style.Font.Bold = true;
    worksheet.Cell("A1").Style.Font.FontSize = 16;
    worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

    worksheet.Cell("A2").Value = "Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    worksheet.Range("A2:E2").Merge();
    worksheet.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

    worksheet.Range("A3:E3").Merge();

    // Table Header Row (row 4)
    worksheet.Cell("A4").Value = "Fund Name";
    worksheet.Cell("B4").Value = "Ticker Code";
    worksheet.Cell("C4").Value = "NAV Price";
    worksheet.Cell("D4").Value = "Market Price";
    worksheet.Cell("E4").Value = "Hold In Trust";

    // Style header row
    var headerRange = worksheet.Range("A4:E4");
    headerRange.Style.Font.Bold = true;
    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

    // Data Rows
    int row = 5;
    foreach (var fund in funds)
    {
        worksheet.Cell(row, 1).Value = fund.FundName;
        worksheet.Cell(row, 2).Value = fund.TickerCode;
        worksheet.Cell(row, 3).Value = fund.NavPrice;
        worksheet.Cell(row, 4).Value = fund.MarketPrice;
        worksheet.Cell(row, 5).Value = fund.HoldInTrust;

        // Center align numeric values
        worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Add borders
        var dataRange = worksheet.Range(row, 1, row, 5);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        row++;
    }

    // Auto-fit columns
    worksheet.Columns().AdjustToContents();

    // Footer Section
    int footerStartRow = row + 2;
    worksheet.Cell(footerStartRow, 1).Value = "END OF REPORT";
    worksheet.Range(footerStartRow, 1, footerStartRow, 5).Merge();
    worksheet.Cell(footerStartRow, 1).Style.Font.Bold = true;
    worksheet.Cell(footerStartRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

    worksheet.Cell(footerStartRow + 1, 1).Value = $"Total Funds: {funds.Count}";
    worksheet.Range(footerStartRow + 1, 1, footerStartRow + 1, 5).Merge();
    worksheet.Cell(footerStartRow + 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

    worksheet.Cell(footerStartRow + 2, 1).Value = "This document is confidential and intended for internal use only.";
    worksheet.Range(footerStartRow + 2, 1, footerStartRow + 2, 5).Merge();
    worksheet.Cell(footerStartRow + 2, 1).Style.Font.Italic = true;
    worksheet.Cell(footerStartRow + 2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

    using var stream = new MemoryStream();
    workbook.SaveAs(stream);
    var excelBytes = stream.ToArray();

    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
}
```

### Option 2: EPPlus (Licensed)

Requires license configuration in `appsettings.json`:
```json
{
  "EPPlus": {
    "ExcelPackage": {
      "License": "NonCommercialPersonal:james"
    }
  }
}
```

Implementation is similar to ClosedXML but uses EPPlus API:
```csharp
using (var package = new ExcelPackage())
{
    var worksheet = package.Workbook.Worksheets.Add("Funds");
    // Similar structure to ClosedXML example
    var excelBytes = await package.GetAsByteArrayAsync();
    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ...);
}
```

**Excel Features** (both versions):
- Merged cells for header and footer
- Auto-fit columns
- Cell borders
- Centered alignment for headers and numeric values
- Bold/italic formatting
- Gray background for header row

---

## 5. Download Token System

### Token Storage: `AppData/download-tokens.json`

```json
{
  "Version": "638800000000000000",
  "Tokens": {
    "pdf": {
      "Token": "44-character-random-string",
      "Expiry": "2025-01-28T12:30:00Z",
      "Type": "pdf"
    },
    "csv": { ... },
    "excel-free": { ... },
    "excel": { ... }
  }
}
```

### Token Service: `DownloadTokenService.cs`

**Key Features**:
- Tokens generated at startup if file doesn't exist
- Refreshed every 20 minutes (background timer)
- Tokens expire after 30 minutes
- Version tracking for frontend sync

**Token Generation**:
```csharp
private static string GenerateRandomToken()
{
    using var rng = RandomNumberGenerator.Create();
    var bytes = new byte[32];
    rng.GetBytes(bytes);
    var base64 = Convert.ToBase64String(bytes)
        .Replace("+", "-")
        .Replace("/", "_")
        .Replace("=", "");
    return base64.Length > 44 ? base64[..44] : base64.PadRight(44, '0');
}

private static string GenerateVersion() => DateTime.UtcNow.Ticks.ToString();
```

**File Paths**:
- Token storage: `AppData/download-tokens.json`
- Auto-created if folder doesn't exist

### Token Validation Attribute: `ValidateDownloadTokenAttribute.cs`

```csharp
public override void OnActionExecuting(ActionExecutingContext context)
{
    var tokenService = context.HttpContext.RequestServices.GetRequiredService<IDownloadTokenService>();

    // Check for download token in query string
    if (context.HttpContext.Request.Query.TryGetValue("token", out var tokenValue))
    {
        if (tokenService.ValidateDownloadToken(tokenValue.ToString(), _downloadType))
        {
            return; // Valid token
        }
    }

    // No valid token - return JSON error
    context.Result = new JsonResult(new { error = "Valid download token required" })
    {
        StatusCode = 401
    };
}
```

**Validation Steps**:
1. Read token from query string
2. Load tokens from `AppData/download-tokens.json`
3. Check if token type exists
4. Check if token matches stored value
5. Check if token has expired

---

## 6. Frontend Integration

### View: `Views/Shared/Components/FundsTable/Default.cshtml`

#### Download Buttons
```html
<a href="#" id="download-pdf" class="btn btn-danger">Download PDF</a>
<a href="#" id="download-csv" class="btn btn-primary">Download CSV</a>
<a href="#" id="download-excel-free" class="btn btn-success">Download Excel (Free)</a>
<a href="#" id="download-excel-epplus" class="btn btn-success">Download Excel (EPPlus)</a>
```

#### JavaScript Token Management

```javascript
let currentTokenVersion = '';

// Update download links with new tokens
function updateDownloadLinks(tokens) {
    document.getElementById('download-pdf').href = '/funds/exporttopdf?token=' + tokens.pdf;
    document.getElementById('download-csv').href = '/funds/exporttocsv?token=' + tokens.csv;
    document.getElementById('download-excel-free').href = '/funds/exporttoexcel-free?token=' + tokens.excel;
    document.getElementById('download-excel-epplus').href = '/funds/exporttoexcel?token=' + tokens.excelEpPlus;
}

// Fetch tokens from server
function fetchAndUpdateTokens() {
    fetch('/funds/download-tokens')
        .then(response => response.json())
        .then(tokens => {
            currentTokenVersion = tokens.version;
            updateDownloadLinks(tokens);
        });
}

// Check for token refresh every 30 seconds
function checkTokenVersion() {
    fetch('/funds/token-version')
        .then(response => response.json())
        .then(data => {
            if (data.version !== currentTokenVersion) {
                fetchAndUpdateTokens();
            }
        });
}

// Initial load
fetchAndUpdateTokens();

// Poll for changes
setInterval(checkTokenVersion, 30000);
```

### Token Endpoints

**Get Tokens**: `GET /funds/download-tokens`
- Returns all tokens with version
- Anonymous access (in excluded paths)

**Get Version**: `GET /funds/token-version`
- Returns current version only
- Used for polling to detect token refresh

---

## 7. Token Lifecycle & Timing

```
┌─────────────────────────────────────────────────────────────┐
│                    TOKEN LIFECYCLE                           │
└─────────────────────────────────────────────────────────────┘

0 min: Application starts
       └─ Token file created (if doesn't exist)
       └─ Tokens generated with version V1
       └─ Expiry: 30 minutes

15 min: Frontend table reload
       └─ Fetches current tokens (V1)

20 min: Backend timer fires
       └─ Generates NEW tokens with version V2
       └─ Overwrites AppData/download-tokens.json
       └─ New expiry: 50 minutes

20.5 min: Frontend polls version
       └─ Detects version change (V1 → V2)
       └─ Fetches new tokens (V2)
       └─ Updates download links

30 min: Old tokens would have expired
       └─ But already refreshed at 20 min

40 min: Backend timer fires again
       └─ Generates NEW tokens with version V3
       └─ Cycle continues...
```

**Key Points**:
- Frontend polls every 30 seconds (faster than 20 min refresh)
- Tokens always refreshed before expiry (20 min < 30 min)
- Download links update within 30 seconds of token refresh
- User never experiences expired token error

---

## 8. Security & Access Control

### Download Endpoints (Require Token)

**Excluded from API Key Authentication**:
- `/funds/exporttopdf`
- `/funds/exporttocsv`
- `/funds/exporttoexcel-free`
- `/funds/exporttoexcel`

**Why Excluded**:
- These use token-based auth instead
- Tokens are server-side generated and validated
- Frontend fetches tokens via anonymous endpoint

### Token Distribution Endpoints (Anonymous Access)

**Anonymous Endpoints** (in excluded paths):
- `/funds/download-tokens` - Returns all tokens
- `/funds/token-version` - Returns version for polling
- `/funds/api-key` - Returns API key for UpdateTable

**Security Model**:
- Tokens are short-lived (30 min)
- Tokens are server-side only (not guessable)
- Tokens are rotated every 20 min
- Only valid tokens allow downloads

---

## 9. Configuration

### appsettings.json

```json
{
  "EPPlus": {
    "ExcelPackage": {
      "License": "NonCommercialPersonal:james"
    }
  },
  "DownloadTokens": {
    "ExpiryInMinutes": 30
  },
  "ApiKeyAuthentication": {
    "ExcludedPaths": [
      "/funds/exporttopdf",
      "/funds/exporttocsv",
      "/funds/exporttoexcel-free",
      "/funds/exporttoexcel",
      "/funds/download-tokens",
      "/funds/token-version",
      ...
    ]
  }
}
```

---

## 10. Dependencies

### NuGet Packages

**PDF Generation**:
```xml
<PackageReference Include="PdfSharp" Version="..." />
```

**Excel Generation**:
```xml
<PackageReference Include="ClosedXML" Version="..." />
<PackageReference Include="EPPlus" Version="..." />
```

---

## 11. Error Handling

All export endpoints include:
```csharp
try
{
    // Export logic
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error exporting funds to {Format}: {Message}", format, ex.Message);
    _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
    if (ex.InnerException != null)
    {
        _logger.LogError(ex, "Inner exception: {InnerMessage}", ex.InnerException.Message);
    }
    return StatusCode(500, $"An error occurred: {ex.Message}");
}
```

Token validation includes detailed logging:
- Token type not found
- Token mismatch
- Token expired
- Validation successful

---

## 12. File Locations Summary

| Component | Location |
|-----------|----------|
| Export Controller | `Controllers/FundsController.cs` |
| Token Service | `Services/DownloadTokenService.cs` |
| Token Interface | `Services/IDownloadTokenService.cs` |
| Token Validator | `Authorization/ValidateDownloadTokenAttribute.cs` |
| Token Storage | `AppData/download-tokens.json` (auto-generated) |
| Frontend View | `Views/Shared/Components/FundsTable/Default.cshtml` |
| Configuration | `appsettings.json` |

---

## 13. Testing Endpoints

### Test Token Endpoint
```
GET /funds/test-token?type=pdf
```
- Generates and returns a new token

```
GET /funds/test-token?token=...&type=pdf
```
- Validates a token and returns result

---

## 14. Common Issues & Solutions

**Issue**: "Valid download token required"
- **Solution**: Check token version, verify token not expired, check logs

**Issue**: Tokens not refreshing
- **Solution**: Check timer is running, verify AppData folder is writable

**Issue**: PDF export fails
- **Solution**: Verify PdfSharp is installed, check font availability

**Issue**: Excel export shows "license required"
- **Solution**: Configure EPPlus license in appsettings.json

**Issue**: Download links not updating
- **Solution**: Check browser console for errors, verify API calls are succeeding
