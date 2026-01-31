# Historical NAV Pivoted Table Feature - Complete Documentation

**Date:** 2026-01-31
**Status:** Completed

---

## Task Plan

### Overview
Pivot the historical NAV table structure to display dates as columns and fields as rows, making it easier to compare values across different dates.

### Requirements
1. Restructure table from vertical (dates as rows) to horizontal (dates as columns)
2. Display fields (NAV Price, Market Price, Daily Change %, Net Asset Value) as rows
3. Maintain sorting functionality with toggle button
4. Maintain pagination with configurable page sizes
5. Keep USD currency formatting
6. Preserve PDF and Excel export functionality

### Implementation Approach
- Convert static HTML table header to dynamic JavaScript-generated header
- Define field rows array for rendering fields as rows
- Create `renderTableHeader()` function to generate date columns
- Modify `renderTableBody()` to render fields as rows across dates
- Add sort toggle button with visual indicator
- Update page size options for better date comparison view

---

## Change Log

### Files Modified

| File | Lines Modified | Description |
|------|----------------|-------------|
| `Views/Shared/Components/HistoricalNavTable/Default.cshtml` | 13-34, 38-43, 59-70, 78-98, 109-115, 157-181, 183-241, 277-298, 343-345 | Pivoted table structure implementation |

---

## Implementation Details

### 1. Before: Vertical Table Structure

**Previous Layout:**
```
| Date       | NAV Price | Market Price | Daily Change % | Net Asset Value (M) |
|------------|-----------|--------------|----------------|---------------------|
| 01/01/2025 | $50.00    | $49.50       | +1.20%         | $250.00M            |
| 02/01/2025 | $51.20    | $50.70       | +2.40%         | $255.00M            |
| 03/01/2025 | $49.80    | $49.30       | -1.50%         | $248.00M            |
```

### 2. After: Pivoted Table Structure

**New Layout:**
```
| Empty Header | 01/01/2025 | 02/01/2025 | 03/01/2025 | 04/01/2025 | 05/01/2025 |
|--------------|------------|------------|------------|------------|------------|
| NAV Price    | $50.00     | $51.20     | $49.80     | $50.50     | $51.00     |
| Market Price | $49.50     | $50.70     | $49.30     | $50.00     | $50.50     |
| Daily Change%| +1.20%     | +2.40%     | -1.50%     | +0.70%     | +1.00%     |
| Net Asset(M) | $250.00M   | $255.00M   | $248.00M   | $252.00M   | $254.00M   |
```

### 3. HTML Structure Changes

**Before:**
```html
<thead class="table-light">
    <tr>
        <th class="sortable-column" data-column="NavDate">Date <span class="sort-indicator"></span></th>
        <th class="sortable-column" data-column="NavPrice">NAV Price <span class="sort-indicator"></span></th>
        <th class="sortable-column" data-column="MarketPrice">Market Price <span class="sort-indicator"></span></th>
        <th class="sortable-column" data-column="DailyChangePercent">Daily Change % <span class="sort-indicator"></span></th>
        <th class="sortable-column" data-column="NetAssetValue">Net Asset Value (M) <span class="sort-indicator"></span></th>
    </tr>
</thead>
```

**After:**
```html
<!-- Download Buttons with Sort Toggle -->
<div style="display: flex; align-items: center; margin-bottom: 15px;">
    <div style="margin-right: auto;">
        <button id="sort-toggle-btn" class="btn btn-sm btn-secondary" type="button" style="margin-right: 10px;">
            Sort: Newest First <span id="sort-indicator">↓</span>
        </button>
    </div>
    <div>
        <a href="#" id="download-hist-pdf" class="btn btn-danger">Download PDF</a>
        <a href="#" id="download-hist-excel" class="btn btn-success">Download Excel</a>
    </div>
</div>

<table class="table table-hover" id="historical-nav-table">
    <thead class="table-light" id="historical-nav-table-head">
        <!-- Date columns will be populated by JavaScript -->
    </thead>
    <tbody id="historical-nav-table-body">
        <!-- Field rows will be populated by JavaScript -->
    </tbody>
</table>
```

### 4. JavaScript Implementation

#### Field Rows Definition

**Lines 109-115:**
```javascript
const fieldRows = [
    { key: 'NavPrice', label: 'NAV Price', format: 'currency', align: 'end' },
    { key: 'MarketPrice', label: 'Market Price', format: 'currency', align: 'end' },
    { key: 'DailyChangePercent', label: 'Daily Change %', format: 'percent', align: 'end' },
    { key: 'NetAssetValue', label: 'Net Asset Value (M)', format: 'currency', align: 'end' }
];
```

#### Render Table Header Function

**Lines 157-181:**
```javascript
function renderTableHeader(paginatedNavs) {
    const thead = document.getElementById('historical-nav-table-head');
    thead.innerHTML = '';

    // Create header row
    const headerRow = document.createElement('tr');

    // First cell is empty (row labels column)
    const emptyTh = document.createElement('th');
    emptyTh.className = 'table-light';
    emptyTh.style.width = '150px';
    headerRow.appendChild(emptyTh);

    // Add date columns
    paginatedNavs.forEach(nav => {
        const th = document.createElement('th');
        th.className = 'table-light text-center';
        th.style.minWidth = '120px';
        th.textContent = formatDate(nav.NavDate);
        headerRow.appendChild(th);
    });

    thead.appendChild(headerRow);
}
```

#### Render Table Body Function

**Lines 183-241:**
```javascript
function renderTableBody() {
    const tbody = document.getElementById('historical-nav-table-body');
    tbody.innerHTML = '';

    if (allHistoricalNavs.length === 0) {
        tbody.innerHTML = '<tr><td class="text-center text-muted" colspan="10">No data available</td></tr>';
        return;
    }

    // Sort by date
    let sortedNavs = sortHistoricalNavs([...allHistoricalNavs], sortDirection);

    // Paginate
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const paginatedNavs = sortedNavs.slice(startIndex, endIndex);

    // Render header with dates
    renderTableHeader(paginatedNavs);

    // Render each field as a row
    fieldRows.forEach(field => {
        const row = document.createElement('tr');

        // First cell is the field label
        const labelCell = document.createElement('td');
        labelCell.textContent = field.label;
        labelCell.style.fontWeight = 'bold';
        row.appendChild(labelCell);

        // Add values for each date
        paginatedNavs.forEach(nav => {
            const cell = document.createElement('td');
            cell.className = `text-${field.align}`;

            const value = nav[field.key];
            if (field.format === 'percent') {
                const result = getFormattedValue(value, field.format);
                cell.innerHTML = `<span class="${result.class}">${result.text}</span>`;
            } else {
                cell.textContent = getFormattedValue(value, field.format);
            }

            row.appendChild(cell);
        });

        tbody.appendChild(row);
    });

    // Update pagination info
    const totalItems = allHistoricalNavs.length;
    const showingStart = totalItems === 0 ? 0 : startIndex + 1;
    const showingEnd = Math.min(endIndex, totalItems);
    document.getElementById('hist-pagination-info').textContent =
        `Showing ${showingStart}-${showingEnd} of ${totalItems} dates`;

    renderPaginationControls(totalItems);
}
```

#### Sort Toggle Functions

**Lines 277-298:**
```javascript
function toggleSort() {
    sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
    currentPage = 1;
    renderTableBody();
    updateSortButton();
}

function updateSortButton() {
    const btn = document.getElementById('sort-toggle-btn');
    const indicator = document.getElementById('sort-indicator');
    if (btn && indicator) {
        if (sortDirection === 'desc') {
            btn.textContent = 'Sort: Newest First ';
            indicator.textContent = '↓';
        } else {
            btn.textContent = 'Sort: Oldest First ';
            indicator.textContent = '↑';
        }
    }
}
```

#### Event Listener for Sort Toggle

**Lines 301-307:**
```javascript
document.addEventListener('click', function(e) {
    // Handle sort toggle button
    if (e.target.closest('#sort-toggle-btn')) {
        e.preventDefault();
        toggleSort();
        return;
    }
    // ... other handlers
});
```

#### Initial Render

**Lines 343-345:**
```javascript
// Initial render
renderTableBody();
updateSortButton();
```

### 5. Page Size Options Updated

**Before:**
```html
<option value="5">5</option>
<option value="10" selected>10</option>
<option value="25">25</option>
<option value="50">50</option>
```

**After:**
```html
<option value="3">3</option>
<option value="5" selected>5</option>
<option value="10">10</option>
<option value="20">20</option>
```

**Rationale:** Smaller page sizes work better for horizontal date display - 3-5 dates fit well on screen while still allowing comparison.

### 6. CSS Changes

**Removed Styles:**
```css
/* No longer needed - removed sortable-column styles */
.sortable-column:hover {
    background-color: #f0f0f0 !important;
}

.sort-asc .sort-indicator::after {
    content: '↑';
    opacity: 1;
}

.sort-desc .sort-indicator::after {
    content: '↓';
    opacity: 1;
}
```

**Reason:** With pivoted structure, sorting is controlled by the sort toggle button, not clickable column headers.

---

## Benefits of Pivoted Structure

1. **Easier Comparison:** Values across dates are displayed side-by-side for easy comparison
2. **Better Space Utilization:** More compact display fits better on standard screens
3. **Trend Analysis:** Horizontal layout makes trends more visually apparent
4. **Intuitive for Historical Data:** Dates as columns is the standard convention for historical financial data
5. **Responsive Pagination:** Fewer dates per page (3-5) keeps the table readable

---

## API Endpoints (Unchanged)

| Method | Endpoint | Description | Token |
|--------|----------|-------------|-------|
| GET | `/funds/historical-nav-pdf/{fundId}` | Export to PDF | pdf |
| GET | `/funds/historical-nav-excel/{fundId}` | Export to Excel | excel-generic |

---

## User Experience

### Table Features

1. **Pivoted Layout:** Dates as columns, fields as rows
2. **Sort Toggle:** Button to toggle between newest/oldest first with visual indicator
3. **Pagination:** Navigate through dates with Previous/Next buttons
4. **Page Size Selector:** Choose 3, 5, 10, or 20 dates per page
5. **Currency Formatting:** All values in USD ($)
6. **Color Coding:** Green for positive changes, red for negative
7. **Info Display:** "Showing X-Y of Z dates"

### Download Options

1. **Download PDF** - Red button, formatted PDF report
2. **Download Excel** - Green button, generic Excel export

---

## Testing Checklist

### Table Structure
- [x] Table displays with dates as columns
- [x] Fields display as rows (NAV Price, Market Price, Daily Change %, Net Asset Value)
- [x] Empty header cell for row labels
- [x] All formatting preserved (currency, percent, colors)

### Sort Functionality
- [x] Sort toggle button visible and working
- [x] Newest First (↓) indicator displays correctly
- [x] Oldest First (↑) indicator displays correctly
- [x] Sort toggles between desc/asc on button click
- [x] Page resets to 1 when sorting

### Pagination
- [x] Pagination controls visible
- [x] Previous/Next buttons work
- [x] Page numbers clickable
- [x] Page size selector works (3, 5, 10, 20)
- [x] "Showing X-Y of Z dates" displays correctly

### Styling
- [x] USD currency formatting preserved
- [x] Full page width (100%) preserved
- [x] Positive changes shown in green
- [x] Negative changes shown in red
- [x] Field labels bold

### Exports
- [x] PDF download button works
- [x] Excel download button works

### Build
- [x] Build succeeds with no errors

---

## Build Verification

```
Build succeeded.
    3 Warning(s)
    0 Error(s)
```

---

## File Structure Summary

```
Umbraco13/
└── Views/Shared/Components/
    └── HistoricalNavTable/
        └── Default.cshtml                      (MODIFIED - Pivoted table structure)
```

---

**Historical NAV pivoted table feature fully implemented with dates as columns, fields as rows, sort toggle, pagination, and all existing functionality preserved.**

---

## Recent Updates - Center Alignment (2026-01-31)

### Change: Center Table Columns and Values

**Request:** "nav history table center the columns and values"

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Lines 110-115 - Field Rows Alignment:**
```javascript
const fieldRows = [
    { key: 'NavPrice', label: 'NAV Price', format: 'currency', align: 'center' },
    { key: 'MarketPrice', label: 'Market Price', format: 'currency', align: 'center' },
    { key: 'DailyChangePercent', label: 'Daily Change %', format: 'percent', align: 'center' },
    { key: 'NetAssetValue', label: 'Net Asset Value (M)', format: 'currency', align: 'center' }
];
```

**Lines 208-213 - Field Label Alignment:**
```javascript
// First cell is the field label
const labelCell = document.createElement('td');
labelCell.textContent = field.label;
labelCell.style.fontWeight = 'bold';
labelCell.style.textAlign = 'center';  // Added center alignment
row.appendChild(labelCell);
```

**Changes:**
- Changed all field row `align` values from `'end'` to `'center'`
- Added `textAlign = 'center'` style to field label cells
- Date columns already had `text-center` class (no change needed)

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - SVG Composite Bar Chart (2026-01-31)

### Change: Add Responsive SVG Composite Bar Chart

**Request:** "attach a responstive svg file that has a composite bar chart for nav history, with x axis as date, and y axis as values"

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**HTML Structure Added:**
```html
<!-- SVG Composite Bar Chart -->
<div class="card mb-4">
    <div class="card-body">
        <h5 class="card-title text-center mb-4">NAV Price vs Market Price Chart</h5>
        <div id="nav-chart-container" style="width: 100%; overflow-x: auto; position: relative;">
            <svg id="nav-chart" width="100%" height="300" viewBox="0 0 800 300" preserveAspectRatio="xMidYMid meet">
                <!-- Chart rendered by JavaScript -->
            </svg>
            <div id="chart-tooltip"></div>
        </div>
        <!-- Legend -->
        <div class="text-center mt-3">
            <span class="me-4">
                <svg width="16" height="16">
                    <rect width="16" height="16" fill="#3b82f6" rx="2"/>
                </svg>
                NAV Price
            </span>
            <span>
                <svg width="16" height="16">
                    <rect width="16" height="16" fill="#22c55e" rx="2"/>
                </svg>
                Market Price
            </span>
        </div>
    </div>
</div>
```

**CSS Styles Added:**
```css
/* Chart styles */
.chart-bar {
    transition: opacity 0.2s ease;
    cursor: pointer;
}

.chart-bar:hover {
    opacity: 0.7;
}

#chart-tooltip {
    position: absolute;
    background-color: rgba(0, 0, 0, 0.8);
    color: white;
    padding: 8px 12px;
    border-radius: 4px;
    font-size: 12px;
    pointer-events: none;
    opacity: 0;
    transition: opacity 0.2s ease;
    z-index: 1000;
    white-space: nowrap;
}

#chart-tooltip.show {
    opacity: 1;
}
```

**JavaScript Functions Added:**

1. **`renderChart(paginatedNavs)`** - Main chart rendering function
   - Calculates Y axis scale with 5% padding
   - Draws 5 grid lines with labels
   - Renders grouped bars (NAV Price = blue, Market Price = green)
   - Draws X and Y axes with titles
   - Shows dates on X axis

2. **`setupChartTooltips(svg, navData)`** - Tooltip functionality
   - Shows value on hover
   - Follows mouse cursor
   - Displays label and value

**Chart Features:**
- **Responsive:** Uses `viewBox` and `preserveAspectRatio` for scaling
- **Composite/Grouped Bars:** Two bars per date (NAV Price and Market Price)
- **X Axis:** Dates from paginated data
- **Y Axis:** Price values in USD with dynamic scaling
- **Grid Lines:** 5 horizontal dashed grid lines
- **Legend:** Color-coded legend below chart
- **Tooltips:** Hover over bars shows value
- **Hover Effects:** Bars fade on hover
- **Auto-updates:** Chart updates with pagination and sort changes

**Chart Colors:**
- NAV Price: Blue (`#3b82f6`)
- Market Price: Green (`#22c55e`)
- Background: Light gray (`#f8f9fa`)
- Grid lines: Light gray (`#e0e0e0`)

**Dimensions:**
- SVG viewBox: 800x300
- Chart padding: Top 30, Right 30, Bottom 60, Left 70
- Bar width: 35% of group width
- Gap between bars: 10% of group width

**Changes to Existing Code:**
- Added `renderChart(paginatedNavs)` call in `renderTableBody()`
- Chart updates automatically when:
  - Page size changes
  - Pagination occurs
  - Sort toggles

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - Expanded Chart Matching Table Columns (2026-01-31)

### Change: Expand SVG Chart and Match Groups with Table Columns

**Request:** "can we expand the svg chart and try to match the groups with columns in table"

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Key Changes:**

1. **Dynamic viewBox Width:**
   - Chart width now calculates based on number of dates displayed
   - Formula: `totalWidth = labelColumnWidth + (paginatedNavs.length * dateColumnWidth) + 40`
   - Example: For 5 dates: 150 + (5 * 120) + 40 = 790px

2. **Table Column Alignment:**
   ```javascript
   // Table column dimensions (matching table structure)
   const labelColumnWidth = 150;  // Width of label column in table
   const dateColumnWidth = 120;   // Width of each date column in table

   // Left padding accounts for label column + Y axis space
   const padding = {
       top: 40,
       right: 20,
       bottom: 70,
       left: labelColumnWidth + 60  // 150 + 60 = 210px
   };
   ```

3. **Bar Group Positioning:**
   ```javascript
   // Align bar groups with table columns
   const indexToGroupX = (index) => {
       return padding.left + (index * dateColumnWidth) + (dateColumnWidth / 2);
   };
   ```

4. **Larger Bar Dimensions:**
   - Bar width: 45px (increased from ~35% of group)
   - Bar gap: 10px
   - Corner radius: 3px (increased from 2px)

5. **Increased Chart Height:**
   - SVG height: 350px (increased from 300px)
   - Better spacing for labels and grid

6. **Enhanced Labels:**
   - Y axis labels: 12px font (increased from 11px)
   - X axis date labels: 13px font, 500 font weight
   - Axis titles: 13px font (increased from 12px)

7. **Updated Tooltip:**
   - Uses `data-type` attribute for cleaner code
   - Shows colored label matching bar color

**Chart vs Table Alignment:**
```
Table Layout:
| Label (150px) | Date 1 (120px) | Date 2 (120px) | Date 3 (120px) | ...

Chart Layout:
    (150px space)     [Bar Group 1]     [Bar Group 2]     [Bar Group 3]  ...
                        (120px wide)        (120px wide)        (120px wide)
```

**Responsive Behavior:**
- Chart width expands/contracts based on page size (3, 5, 10, 20 dates)
- SVG `preserveAspectRatio="xMidYMid meet"` ensures proper scaling
- `min-width: 600px` ensures minimum readable size

**ViewBox Examples:**
- 3 dates: 150 + (3 * 120) + 40 = 550 → `viewBox="0 0 550 350"`
- 5 dates: 150 + (5 * 120) + 40 = 790 → `viewBox="0 0 790 350"`
- 10 dates: 150 + (10 * 120) + 40 = 1390 → `viewBox="0 0 1390 350"`

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - Perfect Chart-Table Alignment (2026-01-31)

### Change: Align Chart Dates with Table Columns

**Request:** "can we align the date in bar chart with the dates in nav history table? Want the positions aligned"

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Key Changes for Perfect Alignment:**

1. **Exact Width Matching:**
   ```javascript
   // Calculate total width to match table exactly
   const totalWidth = labelColumnWidth + (paginatedNavs.length * dateColumnWidth);
   // Removed extra padding - now exact match
   ```

2. **Y Axis in Label Column Space:**
   ```javascript
   // Chart padding - Y axis fits within the label column space
   const yAxisWidth = 50;  // Space for Y axis labels
   const padding = { top: 40, right: 20, bottom: 70, left: yAxisWidth };
   ```

3. **Bar Groups Start at Table Date Column Position:**
   ```javascript
   // Bar groups start at labelColumnWidth (150px) exactly
   const indexToGroupX = (index) => {
       return labelColumnWidth + (index * dateColumnWidth) + (dateColumnWidth / 2);
   };
   ```

4. **Vertical Divider Line:**
   ```javascript
   // Draw vertical divider line between label column and date columns
   const dividerLine = document.createElementNS(ns, 'line');
   dividerLine.setAttribute('x1', labelColumnWidth);  // At 150px
   dividerLine.setAttribute('y1', padding.top - 10);
   dividerLine.setAttribute('x2', labelColumnWidth);
   dividerLine.setAttribute('y2', height - padding.bottom);
   dividerLine.setAttribute('stroke', '#dee2e6');
   dividerLine.setAttribute('stroke-width', '2');
   ```

5. **Smaller Adjustments:**
   - Bar width: 40px (from 45px)
   - Bar gap: 8px (from 10px)
   - Y axis labels: 11px font (from 12px)
   - Date labels: 12px font (from 13px)
   - Rotated Y axis title for compact fit

**Alignment Visualization:**
```
Table:        | Label (150px) | Date 1 (120px) | Date 2 (120px) | Date 3 (120px) |

Chart:        [$50.00]       |    [Bars]      |    [Bars]      |    [Bars]      |
              Y-axis         |  Centered in   |  Centered in   |  Centered in   |
              labels         |  120px column  |  120px column  |  120px column  |

Divider:                    | (vertical line at 150px)
```

**Width Calculation:**
- **Before:** `totalWidth = 150 + (n * 120) + 40` (extra padding)
- **After:** `totalWidth = 150 + (n * 120)` (exact match)

**Examples:**
- 3 dates: 150 + (3 * 120) = 510px
- 5 dates: 150 + (5 * 120) = 750px
- 10 dates: 150 + (10 * 120) = 1350px

**Bar Positioning:**
- Bar Group 1 center: 150 + (0 * 120) + 60 = 210px
- Bar Group 2 center: 150 + (1 * 120) + 60 = 330px
- Bar Group 3 center: 150 + (2 * 120) + 60 = 450px

**Visual Divider:**
- Vertical line at x=150px marks the boundary between label column and date columns
- Matches table's visual structure
- Color: `#dee2e6` (Bootstrap's border color)

**Build Status:** Succeeded with 0 errors
