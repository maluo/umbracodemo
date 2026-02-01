# Generic PDF Export Service - Usage Guide

## Overview

The `IPdfExportService` is a generic, reusable service for exporting any list of objects to PDF. It handles pagination, headers, footers, and column width calculation automatically.

## Basic Usage

### Example 1: Export Fund Data (as used in FundsController)

```csharp
public class FundsController : UmbracoController
{
    private readonly IPdfExportService _pdfExportService;

    public FundsController(IPdfExportService pdfExportService)
    {
        _pdfExportService = pdfExportService;
    }

    [HttpGet("exporttopdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        var funds = await _fundService.GetAllFundsAsync();

        // Define columns
        var columns = new List<PdfColumnDefinition>
        {
            new() { PropertyName = "FundName", HeaderText = "Fund Name", Alignment = XStringAlignment.Near },
            new() { PropertyName = "TickerCode", HeaderText = "Ticker Code", Width = 100 },
            new() { PropertyName = "NavPrice", HeaderText = "NAV Price", Width = 80, Format = "F2" },
            new() { PropertyName = "MarketPrice", HeaderText = "Market Price", Width = 80, Format = "F2" },
            new() { PropertyName = "HoldInTrust", HeaderText = "Hold In Trust", Width = 100 }
        };

        // Configure options
        var options = new PdfExportOptions
        {
            ReportTitle = "FUND SUMMARY REPORT",
            Subtitle = $"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            ItemsPerPage = 25
        };

        var pdfBytes = _pdfExportService.ExportToPdf(funds, columns, options);

        return File(pdfBytes, "application/pdf", $"Funds_Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
    }
}
```

### Example 2: Export User Data

```csharp
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

// In your controller:
var users = _userService.GetAllUsers();

var columns = new List<PdfColumnDefinition>
{
    new() { PropertyName = "Id", HeaderText = "ID", Width = 50 },
    new() { PropertyName = "FirstName", HeaderText = "First Name" },
    new() { PropertyName = "LastName", HeaderText = "Last Name" },
    new() { PropertyName = "Email", HeaderText = "Email Address" },
    new()
    {
        PropertyName = "CreatedDate",
        HeaderText = "Created",
        Width = 100,
        Format = "yyyy-MM-dd"
    },
    new()
    {
        PropertyName = "IsActive",
        HeaderText = "Active",
        Width = 60,
        CustomFormatter = (value) => (bool)value ? "Yes" : "No"
    }
};

var options = new PdfExportOptions
{
    ReportTitle = "USER LIST",
    ItemsPerPage = 30
};

var pdfBytes = _pdfExportService.ExportToPdf(users, columns, options);
```

### Example 3: Export Order Data with Custom Formatting

```csharp
public class Order
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime OrderDate { get; set; }
}

// In your controller:
var orders = _orderService.GetRecentOrders();

var columns = new List<PdfColumnDefinition>
{
    new() { PropertyName = "OrderId", HeaderText = "Order #", Width = 80 },
    new() { PropertyName = "CustomerName", HeaderText = "Customer", Alignment = XStringAlignment.Near },
    new()
    {
        PropertyName = "Amount",
        HeaderText = "Amount",
        Width = 100,
        Format = "C2",  // Currency format
        CustomFormatter = (value) => ((decimal)value).ToString("C2")  // $123.45
    },
    new()
    {
        PropertyName = "Status",
        HeaderText = "Status",
        Width = 100,
        CustomFormatter = (value) =>
        {
            return value?.ToString()?.ToUpper() switch
            {
                "PENDING" => "⏳ Pending",
                "COMPLETED" => "✓ Completed",
                "CANCELLED" => "✗ Cancelled",
                _ => value?.ToString() ?? ""
            };
        }
    },
    new()
    {
        PropertyName = "OrderDate",
        HeaderText = "Order Date",
        Width = 100,
        Format = "MM/dd/yyyy"
    }
};

var options = new PdfExportOptions
{
    ReportTitle = "RECENT ORDERS",
    Subtitle = $"As of {DateTime.Now:yyyy-MM-dd}",
    ItemsPerPage = 20
};

var pdfBytes = _pdfExportService.ExportToPdf(orders, columns, options);
```

## PdfColumnDefinition Options

| Property | Type | Description |
|----------|------|-------------|
| `PropertyName` | string | Property name in the data object (case-insensitive) |
| `HeaderText` | string | Header text to display for this column |
| `Width` | double | Fixed width (0 = auto-calculate based on content) |
| `Format` | string? | .NET format string (e.g., "C2", "F2", "yyyy-MM-dd") |
| `CustomFormatter` | Func<object?, string>? | Custom function to format values |
| `Alignment` | XStringAlignment | Text alignment (Near, Center, Far) |

## PdfExportOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ReportTitle` | string | "Report" | Main title on first page |
| `Subtitle` | string? | null | Subtitle below title |
| `PageSize` | PageSize | A4 | PDF page size |
| `MarginLeft` | double | 50 | Left margin |
| `MarginTop` | double | 100 | Top margin |
| `MarginBottom` | double | 50 | Bottom margin |
| `RowHeight` | double | 25 | Height of each row |
| `ItemsPerPage` | int | 25 | Items per page |
| `FontFamily` | string | "Arial" | Font family name |
| `FontSize` | double | 10 | Regular text font size |
| `HeaderFontSize` | double | 10 | Header font size |
| `TitleFontSize` | double | 20 | Title font size |
| `ShowPageNumbers` | bool | true | Show page numbers |
| `ShowProgressIndicator` | bool | true | Show "Showing X-Y of Z" |
| `ShowReportFooter` | bool | true | Show "END OF REPORT" footer |
| `FooterText` | string? | null | Confidentiality notice text |
| `GetTotalCount` | Func<int>? | null | Callback to get total count |

## Advanced Features

### Auto-Sizing Columns
Set `Width = 0` to auto-size based on content:
```csharp
new() { PropertyName = "Description", HeaderText = "Description", Width = 0 }
```

### Custom Formatters
Use custom formatter for complex formatting:
```csharp
new()
{
    PropertyName = "Status",
    HeaderText = "Status",
    CustomFormatter = (value) => {
        var status = (OrderStatus)value;
        return status == OrderStatus.Active ? "● Active" : "○ Inactive";
    }
}
```

### Combining Format and Custom Formatter
```csharp
new()
{
    PropertyName = "Percentage",
    HeaderText = "%",
    Format = "P2",  // Format as percentage
    CustomFormatter = (value) =>
    {
        var pct = (decimal)value;
        return pct > 0.5m ? $"{pct:P2} ✓" : $"{pct:P2}";
    }
}
```

## Registration in Program.cs

```csharp
builder.Services.AddScoped<IPdfExportService, PdfExportService>();
```

## File Locations

- **Interface:** `Services/IPdfExportService.cs`
- **Implementation:** `Services/PdfExportService.cs`
- **Models:** `Services/PdfColumnDefinition.cs`, `Services/PdfExportOptions.cs` (defined in PdfExportService.cs)

---

## Change Log

### 2026-01-31: Reduce Empty Space Between Header and Table

**Request:** "for the pdf export remove some empty space between header and main table. But don't make the multi-line headers overlap with the table."

**Changes Made:**

**File:** `Services/PdfExportService.cs`

**1. Dynamic Table Position Calculation (Lines 287-291):**

```csharp
// Before:
DrawPageHeader(page, gfx, currentPageNum, options, font, fontHeader);
var firstPageTableTop = options.MarginTop + 30; // Extra 30 units spacing for first page
DrawTableHeader(gfx, firstPageTableTop, columnWidths, columns, fontBold, options);

// After:
// Draw first page header and calculate where table should start
double headerHeight = DrawPageHeader(page, gfx, currentPageNum, options, font, fontHeader);
// Calculate table position: header end + minimal spacing (15 units)
var firstPageTableTop = headerHeight + 15;
DrawTableHeader(gfx, firstPageTableTop, columnWidths, columns, fontBold, options);
```

**2. Modified DrawPageHeader Return Type (Lines 483-517):**

```csharp
// Before: private void DrawPageHeader(...)
// After:  private double DrawPageHeader(...)

/// <summary>
/// Draw first page header (report title and subtitle)
/// Returns the Y position after the header (header bottom edge)
/// </summary>
private double DrawPageHeader(...)
{
    if (pageNum != 1)
        return 20; // Return minimal position for non-first pages

    // Draw header...
    return yPos; // Return the Y position after all header content
}
```

**Benefits:**

| Before | After |
|--------|-------|
| Fixed spacing: `MarginTop + 30 = 130` | Dynamic: `headerHeight + 15` |
| 60-80 units of empty space | Only 15 units spacing |
| Header overlap risk with multi-line | Automatically adjusts for any header height |

**Space Savings:**

| Header Lines | Before | After | Saved |
|--------------|--------|-------|-------|
| 1 title line | Table at 130 | Table at ~65 | ~50 units |
| 2 title lines | Table at 130 | Table at ~85 | ~30 units |
| With subtitle | Table at 130 | Table at ~110 | ~20 units |

**Build Status:** Succeeded with 0 errors

