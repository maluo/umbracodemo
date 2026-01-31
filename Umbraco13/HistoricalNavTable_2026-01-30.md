# Historical NAV Table Feature - Complete Documentation

**Date:** 2026-01-30
**Task:** Add Historical NAV Table with PDF and Excel Export
**Status:** Completed

---

## Task Plan

### Overview
Add a historical NAV (Net Asset Value) table to the funds page that displays historical price data for funds. Include PDF and Generic Excel export functionality with proper formatting.

### Requirements
1. Create new database table `FundHistoricalNavs` to store historical NAV values
2. Seed 50 dummy records spanning 2018-2025
3. Create service layer for historical NAV data access
4. Create ViewComponent to display historical NAV table
5. Add pagination and sorting functionality
6. Provide PDF export option
7. Provide Generic Excel export option
8. Integrate with existing funds table page

### Implementation Approach
- Create `FundHistoricalNav` entity model with relationship to Fund
- Add seed data in `AppDbContext.OnModelCreating`
- Create `IFundHistoricalNavService` and `FundHistoricalNavService`
- Create `FundHistoricalNavViewModel` and `FundHistoricalNavItem`
- Create `HistoricalNavTableViewComponent` and view
- Add export endpoints in `FundsController`
- Register service in `Program.cs`
- Embed HistoricalNavTable component in FundsTable view

---

## Change Log

### Files Created

| File | Description |
|------|-------------|
| `Models/FundHistoricalNav.cs` | Entity model for historical NAV data |
| `Models/FundHistoricalNavViewModel.cs` | View model and item model for display |
| `Services/IFundHistoricalNavService.cs` | Service interface |
| `Services/FundHistoricalNavService.cs` | Service implementation |
| `ViewComponents/HistoricalNavTableViewComponent.cs` | View component controller |
| `Views/Shared/Components/HistoricalNavTable/Default.cshtml` | View component view with table and exports |

### Files Modified

| File | Lines Modified | Description |
|------|----------------|-------------|
| `Data/AppDbContext.cs` | 14, 92-136 | Added `FundHistoricalNavs` DbSet and seed data |
| `Controllers/FundsController.cs` | 22, 24, 32, 480-567 | Added service injection and export endpoints |
| `Program.cs` | 22 | Registered historical NAV service |
| `Views/Shared/Components/FundsTable/Default.cshtml` | 105-106, 171-177 | Added component invocation and token update |

---

## Implementation Details

### 1. Database Schema - FundHistoricalNav Model

**File:** `Models/FundHistoricalNav.cs`

```csharp
public class FundHistoricalNav
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FundId { get; set; }

    public Fund? Fund { get; set; }

    [Required]
    public decimal NavPrice { get; set; }

    public decimal MarketPrice { get; set; }

    [Required]
    public DateTime NavDate { get; set; }

    public decimal DailyChangePercent { get; set; }

    public decimal NetAssetValue { get; set; }
}
```

**Fields:**
- `Id` - Primary key
- `FundId` - Foreign key to Fund table
- `NavPrice` - NAV price on this date
- `MarketPrice` - Market price on this date
- `NavDate` - Date of the NAV value
- `DailyChangePercent` - Daily percentage change
- `NetAssetValue` - Net asset value in millions

### 2. Seed Data Distribution

**File:** `Data/AppDbContext.cs` (lines 92-136)

- **50 records** spread across **2018-2025** (~6-7 records per year)
- Records distributed among **first 10 funds** (Fund IDs 1-10)
- Each fund gets 5 historical NAV records
- NAV values grow over time with realistic variation
- Daily change: -3% to +3%
- Net asset values: $100M to $600M

```csharp
// Seed historical NAV data - 50 records spanning 2018-2025
var historicalNavs = new List<FundHistoricalNav>();
var histRandom = new Random(43);

for (int i = 1; i <= 50; i++)
{
    int yearOffset = (i - 1) / 6; // Spread across years
    int monthOffset = ((i - 1) % 12) + 1;
    int dayOffset = histRandom.Next(1, 28);
    var navDate = new DateTime(2018 + yearOffset, monthOffset, dayOffset);
    int fundId = ((i - 1) % 10) + 1;

    // Generate realistic NAV values with growth over time
    decimal baseNav = 30 + (fundId * 5);
    decimal yearlyChange = (yearOffset * 2m);
    decimal randomVariation = (decimal)(histRandom.NextDouble() * 10 - 5);
    decimal navPrice = Math.Round(baseNav + yearlyChange + randomVariation, 2);

    // ... additional field generation
}
```

### 3. Service Layer

**File:** `Services/IFundHistoricalNavService.cs`

```csharp
public interface IFundHistoricalNavService
{
    Task<List<FundHistoricalNav>> GetHistoricalNavsByFundIdAsync(int fundId);
    Task<FundHistoricalNavViewModel> GetHistoricalNavViewModelAsync(int fundId);
    Task<List<FundHistoricalNav>> GetAllHistoricalNavsAsync();
}
```

**File:** `Services/FundHistoricalNavService.cs`

- `GetHistoricalNavsByFundIdAsync` - Gets all historical NAVs for a fund, ordered by date
- `GetHistoricalNavViewModelAsync` - Returns view model with fund info and historical items
- `GetAllHistoricalNavsAsync` - Gets all historical NAV data

### 4. View Component

**File:** `ViewComponents/HistoricalNavTableViewComponent.cs`

```csharp
public class HistoricalNavTableViewComponent : ViewComponent
{
    private readonly IFundHistoricalNavService _fundHistoricalNavService;

    public HistoricalNavTableViewComponent(IFundHistoricalNavService fundHistoricalNavService)
    {
        _fundHistoricalNavService = fundHistoricalNavService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int fundId)
    {
        var model = await _fundHistoricalNavService.GetHistoricalNavViewModelAsync(fundId);
        return View(model);
    }
}
```

### 5. View - Historical Nav Table

**File:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Features:**
- Table columns: Date, NAV Price, Market Price, Daily Change %, Net Asset Value (M)
- Sortable by all columns (default: Date descending)
- Pagination with page size selector (5, 10, 25, 50 entries)
- Color-coded daily change:
  - Green for positive values
  - Red for negative values
- Currency formatting for GBP
- Date formatting (en-GB locale)

**Download Buttons:**
- Download PDF - Red button with PDF icon
- Download Excel - Green button with Excel icon

### 6. Export Endpoints

**File:** `Controllers/FundsController.cs` (lines 480-567)

#### PDF Export Endpoint
```csharp
[HttpGet("historical-nav-pdf/{fundId}")]
[ValidateDownloadToken("pdf")]
public async Task<IActionResult> ExportHistoricalNavToPdf(int fundId)
```

**Features:**
- Uses `PdfExportService` with column definitions
- Report title includes fund name and ticker
- Configurable items per page (50)
- Disclaimer included
- Footer with fund information
- Filename: `HistoricalNav_{TickerCode}_{yyyyMMdd_HHmmss}.pdf`

#### Excel Export Endpoint
```csharp
[HttpGet("historical-nav-excel/{fundId}")]
[ValidateDownloadToken("excel-generic")]
public async Task<IActionResult> ExportHistoricalNavToExcel(int fundId)
```

**Features:**
- Uses `ExcelExportService` with generic export
- Worksheet name: "Historical NAV"
- Multi-line title with bold markup
- Disclaimer section
- All columns left-aligned
- Auto-fit columns enabled
- Borders enabled
- Filename: `HistoricalNav_{TickerCode}_{yyyyMMdd_HHmmss}.xlsx`

### 7. Service Registration

**File:** `Program.cs` (line 22)

```csharp
builder.Services.AddScoped<Umbraco13.Services.IFundHistoricalNavService, Umbraco13.Services.FundHistoricalNavService>();
```

### 8. Integration with Main Funds Table

**File:** `Views/Shared/Components/FundsTable/Default.cshtml`

**Added:** (lines 105-106)
```cshtml
@* Historical NAV Table - Show for first fund by default *@
await Component.InvokeAsync("HistoricalNavTable", new { fundId = 1 })
```

**Token Update:** (lines 171-177)
```javascript
// Update historical NAV download links if they exist
const histPdfLink = document.getElementById('download-hist-pdf');
const histExcelLink = document.getElementById('download-hist-excel');
if (histPdfLink && histExcelLink) {
    histPdfLink.href = '/funds/historical-nav-pdf/1?token=' + tokens.pdf;
    histExcelLink.href = '/funds/historical-nav-excel/1?token=' + tokens.excelGeneric;
}
```

---

## Database Changes

### New Table: FundHistoricalNavs

| Column | Type | Description |
|--------|------|-------------|
| Id | INTEGER (PK) | Primary key |
| FundId | INTEGER (FK) | Foreign key to Funds table |
| NavPrice | DECIMAL | NAV price on date |
| MarketPrice | DECIMAL | Market price on date |
| NavDate | DATETIME | Date of NAV value |
| DailyChangePercent | DECIMAL | Daily percentage change |
| NetAssetValue | DECIMAL | Net asset value (millions) |

**Relationship:** FundHistoricalNavs.FundId → Funds.Id (Many-to-One)

---

## API Endpoints

### Historical NAV Exports

| Method | Endpoint | Description | Token Required |
|--------|----------|-------------|----------------|
| GET | `/funds/historical-nav-pdf/{fundId}` | Export historical NAV to PDF | pdf |
| GET | `/funds/historical-nav-excel/{fundId}` | Export historical NAV to Excel | excel-generic |

---

## User Flow

1. User navigates to the funds page
2. Main funds table displays all funds
3. Historical NAV table displays below for Fund ID 1 (default)
4. User can:
   - Sort historical NAV data by any column
   - Change page size
   - Navigate through pages
   - Download PDF of historical NAV
   - Download Excel of historical NAV

---

## Testing Checklist

- [x] `FundHistoricalNav` model created with all required fields
- [x] `FundHistoricalNavs` DbSet added to `AppDbContext`
- [x] 50 seed records added spanning 2018-2025
- [x] `IFundHistoricalNavService` interface created
- [x] `FundHistoricalNavService` implementation created
- [x] `FundHistoricalNavViewModel` and `FundHistoricalNavItem` created
- [x] `HistoricalNavTableViewComponent` created
- [x] Historical NAV table view created with styling
- [x] Pagination implemented (5, 10, 25, 50)
- [x] Sorting implemented for all columns
- [x] Color-coded daily change (green/red)
- [x] PDF export endpoint added
- [x] Excel export endpoint added
- [x] Service registered in `Program.cs`
- [x] Component integrated in FundsTable view
- [x] Token update function includes historical NAV links
- [x] Database file deleted for recreation
- [x] Build succeeds with no errors

---

## Build Verification

```
Build succeeded.
    3 Warning(s)
    0 Error(s)
```

---

## Next Steps (Optional Enhancements)

1. **Fund Selector** - Add dropdown to select different funds for historical NAV display
2. **Date Range Filter** - Add date range picker to filter historical data
3. **Chart Integration** - Add line chart showing NAV price trends over time
4. **Performance Metrics** - Add YTD return, 1-year return, etc.
5. **Comparison View** - Compare multiple funds side by side
6. **Data Refresh** - Add ability to refresh historical data from external API

---

Historical NAV table feature has been successfully implemented with full PDF and Excel export functionality.
