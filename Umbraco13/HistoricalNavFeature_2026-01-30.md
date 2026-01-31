# Historical NAV Table Feature - Complete Documentation

**Date:** 2026-01-30
**Status:** Completed

---

## Features Implemented

1. **Historical NAV Database Table** - New table with 50 dummy records spanning 2018-2025
2. **Historical NAV Display Table** - Full-featured table with sorting and pagination
3. **PDF Export** - Download historical NAV data as PDF
4. **Generic Excel Export** - Download historical NAV data as Excel
5. **USD Currency Formatting** - All prices displayed in US Dollars
6. **Full Page Width** - Table spans 100% of page width
7. **Clickable Sorting** - Sort by any column with visual indicators
8. **Pagination Controls** - Full pagination with customizable page sizes

---

## Task Plan

### Overview
Add a historical NAV (Net Asset Value) table to the funds page displaying historical price data for funds. Include full sorting, pagination, and PDF/Excel export functionality with USD currency formatting.

### Requirements
1. Create `FundHistoricalNav` database table with proper schema
2. Seed 50 dummy records spanning 2018-2025 across first 10 funds
3. Create service layer for data access
4. Create ViewComponent for table rendering
5. Implement sorting by all columns with visual indicators
6. Implement pagination with configurable page sizes
7. Use USD currency formatting
8. Make table span full page width (100%)
9. Provide PDF export functionality
10. Provide Generic Excel export functionality

### Implementation Approach
- Entity Framework Code First with SQLite
- ViewComponent architecture for reusable table
- Client-side sorting and pagination with JavaScript
- Server-side PDF/Excel export using existing services
- Token-based security for downloads

---

## Change Log

### Files Created

| File | Lines | Description |
|------|-------|-------------|
| `Models/FundHistoricalNav.cs` | 50 | Entity model for historical NAV data |
| `Models/FundHistoricalNavViewModel.cs` | 40 | View model and item model |
| `Services/IFundHistoricalNavService.cs` | 20 | Service interface |
| `Services/FundHistoricalNavService.cs` | 80 | Service implementation |
| `ViewComponents/HistoricalNavTableViewComponent.cs` | 20 | View component controller |
| `Views/Shared/Components/HistoricalNavTable/Default.cshtml` | 325 | Table view with sorting/pagination |
| `HistoricalNavTable_2026-01-30.md` | - | Initial documentation |
| `HistoricalNavFeature_2026-01-30.md` | - | This comprehensive documentation |

### Files Modified

| File | Lines Modified | Description |
|------|----------------|-------------|
| `Data/AppDbContext.cs` | 14, 92-136 | Added DbSet and seed data |
| `Controllers/FundsController.cs` | 22, 24, 32, 480-567 | Added service injection and export endpoints |
| `Program.cs` | 22 | Registered service |
| `Views/Shared/Components/FundsTable/Default.cshtml` | 105-106, 163-180 | Added component and token updates |

---

## Implementation Details

### 1. Database Schema

**Table:** `FundHistoricalNavs`

| Column | Type | Description |
|--------|------|-------------|
| Id | INTEGER (PK) | Primary key |
| FundId | INTEGER (FK) | Foreign key to Funds.Id |
| NavPrice | DECIMAL | NAV price on date |
| MarketPrice | DECIMAL | Market price on date |
| NavDate | DATETIME | Date of NAV value |
| DailyChangePercent | DECIMAL | Daily percentage change |
| NetAssetValue | DECIMAL | Net asset value (millions) |

**Relationship:** FundHistoricalNavs.FundId → Funds.Id (Many-to-One)

### 2. Seed Data Distribution

```csharp
// 50 records spanning 2018-2025
// Distributed across first 10 funds (IDs 1-10)
// Each fund gets 5 historical records
// Years: 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025
// NAV values grow over time with realistic variation
```

**Data Characteristics:**
- Date range: January 2018 - December 2025
- NAV range: ~$30 - $85 depending on fund and growth
- Daily change: -3% to +3%
- Net asset values: $100M - $600M

### 3. Service Layer

**IFundHistoricalNavService Interface:**
```csharp
Task<List<FundHistoricalNav>> GetHistoricalNavsByFundIdAsync(int fundId);
Task<FundHistoricalNavViewModel> GetHistoricalNavViewModelAsync(int fundId);
Task<List<FundHistoricalNav>> GetAllHistoricalNavsAsync();
```

**FundHistoricalNavService Implementation:**
- Fetches historical NAVs ordered by date
- Returns view model with fund info
- Handles errors gracefully

### 4. View Component

**HistoricalNavTableViewComponent:**
```csharp
public async Task<IViewComponentResult> InvokeAsync(int fundId)
{
    var model = await _fundHistoricalNavService.GetHistoricalNavViewModelAsync(fundId);
    return View(model);
}
```

### 5. Table View Features

#### Sorting Implementation

**HTML Headers with Sort Indicators:**
```html
<th class="sortable-column" data-column="NavDate" style="cursor: pointer;">
    Date <span class="sort-indicator"></span>
</th>
```

**Sort Indicators CSS:**
```css
.sort-indicator::after {
    content: '⇅';
    margin-left: 5px;
    opacity: 0.3;
}

.sort-asc .sort-indicator::after {
    content: '↑';
    opacity: 1;
}

.sort-desc .sort-indicator::after {
    content: '↓';
    opacity: 1;
}

.sortable-column:hover {
    background-color: #f0f0f0 !important;
}
```

**JavaScript Click Handler:**
```javascript
document.addEventListener('click', function(e) {
    const sortableColumn = e.target.closest('.sortable-column');
    if (sortableColumn) {
        e.preventDefault();
        const column = sortableColumn.dataset.column;

        if (sortColumn === column) {
            // Toggle direction
            sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
            // New column, default to ascending
            sortColumn = column;
            sortDirection = 'asc';
        }

        currentPage = 1; // Reset to first page when sorting
        renderTableBody();
    }
});
```

#### Pagination Implementation

**HTML Structure:**
```html
<div class="d-flex justify-content-between align-items-center mt-3">
    <div>
        <span id="hist-pagination-info">Showing 1-10 of 50 entries</span>
    </div>
    <nav aria-label="Page navigation">
        <ul class="pagination mb-0" id="hist-pagination-controls">
            <!-- Populated by JavaScript -->
        </ul>
    </nav>
</div>

<div class="mt-3 text-end">
    <label>Show
        <select id="hist-page-size-select" class="form-select form-select-sm d-inline-block">
            <option value="5">5</option>
            <option value="10" selected>10</option>
            <option value="25">25</option>
            <option value="50">50</option>
        </select>
        entries per page
    </label>
</div>
```

**JavaScript Pagination Function:**
```javascript
function renderPaginationControls(totalItems) {
    const totalPages = Math.ceil(totalItems / pageSize);
    const pagination = document.getElementById('hist-pagination-controls');

    if (!pagination) return;

    pagination.innerHTML = '';

    // Always show pagination controls
    // Previous button
    const prevLi = document.createElement('li');
    prevLi.className = `page-item ${currentPage === 1 ? 'disabled' : ''}`;
    prevLi.innerHTML = `<a class="page-link" href="#" data-page="${currentPage - 1}">Previous</a>`;
    pagination.appendChild(prevLi);

    // Page numbers (show max 5 pages)
    let startPage = Math.max(1, currentPage - 2);
    let endPage = Math.min(totalPages, startPage + 4);
    startPage = Math.max(1, endPage - 4);

    for (let i = startPage; i <= endPage; i++) {
        const pageLi = document.createElement('li');
        pageLi.className = `page-item ${i === currentPage ? 'active' : ''}`;
        pageLi.innerHTML = `<a class="page-link" href="#" data-page="${i}">${i}</a>`;
        pagination.appendChild(pageLi);
    }

    // Next button
    const nextLi = document.createElement('li');
    nextLi.className = `page-item ${currentPage === totalPages ? 'disabled' : ''}`;
    nextLi.innerHTML = `<a class="page-link" href="#" data-page="${currentPage + 1}">Next</a>`;
    pagination.appendChild(nextLi);
}
```

#### Currency Formatting

**USD Currency:**
```javascript
function formatCurrency(value) {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
}
```

**Date Formatting:**
```javascript
function formatDate(dateStr) {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-GB');
}
```

#### Color-Coded Daily Change

**CSS:**
```css
.positive-change {
    color: green;
}

.negative-change {
    color: red;
}
```

**JavaScript:**
```javascript
const changeClass = nav.DailyChangePercent >= 0 ? 'positive-change' : 'negative-change';
const changeSign = nav.DailyChangePercent >= 0 ? '+' : '';
// ...
<td class="text-end ${changeClass}">${changeSign}${formatPercentage(nav.DailyChangePercent)}</td>
```

### 6. Export Endpoints

#### PDF Export

**Endpoint:** `GET /funds/historical-nav-pdf/{fundId}`

```csharp
[HttpGet("historical-nav-pdf/{fundId}")]
[ValidateDownloadToken("pdf")]
public async Task<IActionResult> ExportHistoricalNavToPdf(int fundId)
{
    var viewModel = await _fundHistoricalNavService.GetHistoricalNavViewModelAsync(fundId);
    var historicalNavs = await _fundHistoricalNavService.GetHistoricalNavsByFundIdAsync(fundId);

    var columns = new List<PdfColumnDefinition>
    {
        new() { PropertyName = "NavDate", HeaderText = "Date", Width = 80, Format = "d" },
        new() { PropertyName = "NavPrice", HeaderText = "NAV Price", Width = 70, Format = "F2" },
        new() { PropertyName = "MarketPrice", HeaderText = "Market Price", Width = 70, Format = "F2" },
        new() { PropertyName = "DailyChangePercent", HeaderText = "Daily Change %", Width = 70, Format = "F2" },
        new() { PropertyName = "NetAssetValue", HeaderText = "Net Asset Value (M)", Width = 80, Format = "F2" }
    };

    var options = new PdfExportOptions
    {
        ReportTitle = $"HISTORICAL NAV VALUES\n{viewModel.FundName} ({viewModel.TickerCode})",
        ItemsPerPage = 50,
        Disclaimer = "This report contains historical NAV prices and is for informational purposes only.",
        FooterText = $"Fund: {viewModel.FundName} | Ticker: {viewModel.TickerCode}",
        ShowAverageRow = false
    };

    var pdfBytes = _pdfExportService.ExportToPdf(historicalNavs, columns, options);

    return File(pdfBytes, "application/pdf",
        $"HistoricalNav_{viewModel.TickerCode}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
}
```

#### Excel Export

**Endpoint:** `GET /funds/historical-nav-excel/{fundId}`

```csharp
[HttpGet("historical-nav-excel/{fundId}")]
[ValidateDownloadToken("excel-generic")]
public async Task<IActionResult> ExportHistoricalNavToExcel(int fundId)
{
    var viewModel = await _fundHistoricalNavService.GetHistoricalNavViewModelAsync(fundId);
    var historicalNavs = await _fundHistoricalNavService.GetHistoricalNavsByFundIdAsync(fundId);

    var columns = new List<ExcelColumnDefinition>
    {
        new() { PropertyName = "NavDate", HeaderText = "Date", Width = 15, Alignment = ExcelAlignment.Left },
        new() { PropertyName = "NavPrice", HeaderText = "NAV Price", Width = 12, Format = "F2", Alignment = ExcelAlignment.Left },
        new() { PropertyName = "MarketPrice", HeaderText = "Market Price", Width = 12, Format = "F2", Alignment = ExcelAlignment.Left },
        new() { PropertyName = "DailyChangePercent", HeaderText = "Daily Change %", Width = 12, Format = "F2", Alignment = ExcelAlignment.Left },
        new() { PropertyName = "NetAssetValue", HeaderText = "Net Asset Value (M)", Width = 15, Format = "F2", Alignment = ExcelAlignment.Left }
    };

    var options = new ExcelExportOptions
    {
        WorksheetName = "Historical NAV",
        ReportTitle = $"**HISTORICAL NAV VALUES**\n{viewModel.FundName} ({viewModel.TickerCode})",
        Disclaimer = "**Disclaimer:** This report contains historical NAV prices and is for informational purposes only.\nPast performance is not indicative of future results.",
        TitleFont = new ExcelFontStyle { FontSize = 14, Bold = true },
        HeaderFont = new ExcelFontStyle { Bold = true, BackgroundColor = "#D3D3D3" },
        DataFont = new ExcelFontStyle { FontSize = 10 },
        AutoFitColumns = true,
        ShowBorders = true
    };

    var excelBytes = _excelExportService.ExportToExcel(historicalNavs, columns, options);

    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        $"HistoricalNav_{viewModel.TickerCode}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
}
```

### 7. Service Registration

**Program.cs:**
```csharp
builder.Services.AddScoped<Umbraco13.Services.IFundHistoricalNavService, Umbraco13.Services.FundHistoricalNavService>();
```

### 8. Integration with Main Funds Table

**FundsTable/Default.cshtml:**
```cshtml
@* Historical NAV Table - Show for first fund by default *@
@await Component.InvokeAsync("HistoricalNavTable", new { fundId = 1 })
```

**Token Updates:**
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

## Bug Fixes and Improvements

### 1. Pagination Controls Missing

**Problem:** Pagination controls were not visible

**Root Cause:**
- CSS class mismatch: `.historical-pagination` vs actual class `.pagination`
- Early return in `renderPaginationControls()` after clearing innerHTML

**Solution:**
1. Changed CSS from `.historical-pagination` to `.pagination`
2. Added null check before clearing innerHTML
3. Removed early return so pagination always renders

**Code Change:**
```javascript
// Before
if (totalPages <= 1) return;

// After
if (!pagination) return;
// Always render pagination controls
```

### 2. USD Currency Formatting

**Change:** Updated from GBP to USD

**Before:**
```javascript
function formatCurrency(value) {
    return new Intl.NumberFormat('en-GB', { style: 'currency', currency: 'GBP' }).format(value);
}
```

**After:**
```javascript
function formatCurrency(value) {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
}
```

### 3. Full Page Width

**Change:** Width from 90% to 100%

**Before:**
```html
<div id="historical-nav-container" style="margin: 40px auto 0 auto; width: 90%; max-width: 100%;">
```

**After:**
```html
<div id="historical-nav-container" style="margin: 40px auto 0 auto; width: 100%; max-width: 100%;">
```

---

## API Endpoints

| Method | Endpoint | Description | Token |
|--------|----------|-------------|-------|
| GET | `/funds/historical-nav-pdf/{fundId}` | Export to PDF | pdf |
| GET | `/funds/historical-nav-excel/{fundId}` | Export to Excel | excel-generic |

---

## User Experience

### Table Features

1. **Sortable Columns:** Click any header to sort (toggles asc/desc)
2. **Sort Indicators:** Visual feedback (⇅, ↑, ↓)
3. **Pagination:** Navigate through pages with Previous/Next
4. **Page Size Selector:** Choose 5, 10, 25, or 50 entries per page
5. **Currency Formatting:** All values in USD ($)
6. **Color Coding:** Green for positive changes, red for negative
7. **Info Display:** "Showing X-Y of Z entries"

### Download Options

1. **Download PDF** - Red button, formatted PDF report
2. **Download Excel** - Green button, generic Excel export

---

## Testing Checklist

### Database
- [x] FundHistoricalNav model created
- [x] FundHistoricalNavs DbSet added to AppDbContext
- [x] 50 seed records spanning 2018-2025
- [x] Database recreated with new table

### Service Layer
- [x] IFundHistoricalNavService interface created
- [x] FundHistoricalNavService implementation created
- [x] Service registered in Program.cs
- [x] Dependency injection working

### View Component
- [x] HistoricalNavTableViewComponent created
- [x] View model created
- [x] Default.cshtml view created
- [x] Component integrated into FundsTable

### Table Functionality
- [x] Table displays historical NAV data
- [x] All columns sortable with indicators
- [x] Sort toggles between asc/desc
- [x] Pagination controls visible
- [x] Previous/Next buttons work
- [x] Page numbers clickable
- [x] Page size selector works
- [x] "Showing X-Y of Z entries" displays

### Styling
- [x] USD currency formatting
- [x] Full page width (100%)
- [x] Hover effect on sortable columns
- [x] Sort indicators display correctly
- [x] Positive changes shown in green
- [x] Negative changes shown in red

### Exports
- [x] PDF export endpoint created
- [x] Excel export endpoint created
- [x] PDF download button works
- [x] Excel download button works
- [x] Token validation working
- [x] Filenames include ticker code and timestamp

### Build
- [x] Build succeeds with no errors
- [x] All services registered
- [x] No runtime errors

---

## Build Verification

```
Build succeeded.
    3 Warning(s)
    0 Error(s)
```

---

## Future Enhancements

1. **Fund Selector Dropdown** - Allow users to select different funds
2. **Date Range Filter** - Add date picker for filtering by date range
3. **Chart Integration** - Add line chart showing NAV trends over time
4. **Performance Metrics** - Display YTD return, 1-year return, etc.
5. **Comparison View** - Compare multiple funds side by side
6. **Real-time Updates** - Auto-refresh historical data
7. **Export All Funds** - Export historical data for all funds at once

---

## File Structure Summary

```
Umbraco13/
├── Models/
│   ├── FundHistoricalNav.cs              (NEW - Entity model)
│   └── FundHistoricalNavViewModel.cs      (NEW - View models)
├── Services/
│   ├── IFundHistoricalNavService.cs       (NEW - Interface)
│   └── FundHistoricalNavService.cs        (NEW - Implementation)
├── ViewComponents/
│   └── HistoricalNavTableViewComponent.cs (NEW - View component)
├── Views/Shared/Components/
│   └── HistoricalNavTable/
│       └── Default.cshtml                 (NEW - Table view)
├── Controllers/
│   └── FundsController.cs                 (MODIFIED - Added endpoints)
├── Data/
│   └── AppDbContext.cs                     (MODIFIED - Added DbSet and seed)
├── Program.cs                              (MODIFIED - Service registration)
└── Views/Shared/Components/FundsTable/
    └── Default.cshtml                      (MODIFIED - Component invocation)
```

---

## Recent Updates - Pivoted Table Structure (2026-01-31)

### Change: Pivoted Table Display

**Request:** "For nav history display please pivot the table with date in columns, and other fields in rows"

**Implementation:**

1. **Table Structure Changed:**
   - **Before:** Vertical table with dates as rows, fields as columns
   - **After:** Horizontal/pivoted table with dates as columns, fields as rows

2. **New Layout:**
   ```
   | Empty Header | Date 1 | Date 2 | Date 3 | Date 4 | Date 5 |
   |--------------|--------|--------|--------|--------|--------|
   | NAV Price    | $50.00 | $51.20 | $49.80 | $50.50 | $51.00 |
   | Market Price | $49.50 | $50.70 | $49.30 | $50.00 | $50.50 |
   | Daily Change%| +1.20% | +2.40% | -1.50% | +0.70% | +1.00% |
   | Net Asset(M) | $250M  | $255M  | $248M  | $252M  | $254M  |
   ```

3. **Key Changes in `Default.cshtml`:**

   **HTML Structure:**
   - Changed static `<thead>` to dynamic `<thead id="historical-nav-table-head">`
   - Removed sortable-column classes from headers
   - Added sort toggle button with indicator

   **JavaScript Changes:**
   ```javascript
   // Define field rows
   const fieldRows = [
       { key: 'NavPrice', label: 'NAV Price', format: 'currency', align: 'end' },
       { key: 'MarketPrice', label: 'Market Price', format: 'currency', align: 'end' },
       { key: 'DailyChangePercent', label: 'Daily Change %', format: 'percent', align: 'end' },
       { key: 'NetAssetValue', label: 'Net Asset Value (M)', format: 'currency', align: 'end' }
   ];

   // New function to render date columns
   function renderTableHeader(paginatedNavs) {
       // Creates header row with dates as columns
   }

   // Modified renderTableBody to render fields as rows
   function renderTableBody() {
       // Renders each field as a row with values across dates
   }
   ```

4. **Sort Toggle Button:**
   - Added button to toggle date sort order (newest first/oldest first)
   - Visual indicator: ↓ for newest first, ↑ for oldest first
   - Positioned on left side of download buttons

5. **Page Size Options Updated:**
   - Changed from 5, 10, 25, 50 to 3, 5, 10, 20 (dates per page)

6. **Pagination Info Text:**
   - Changed from "entries" to "dates" for clarity

**Benefits:**
- Easier comparison of values across different dates
- More compact display for screen width
- Better for trend analysis side-by-side
- Dates as columns is more intuitive for historical data

---

**Historical NAV table feature fully implemented with sorting, pagination, USD formatting, full page width, pivoted table structure, and PDF/Excel export functionality.**
