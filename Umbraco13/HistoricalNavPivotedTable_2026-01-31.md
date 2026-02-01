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

---

## Recent Updates - Y Axis at Label Column Boundary (2026-01-31)

### Change: Move Y Axis to Match Table Structure Exactly

**Request:** "nav history, align bar chart groups with the match date in table columns"

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Key Change - Y Axis Position:**

**Before:**
```javascript
const yAxisWidth = 50;  // Space for Y axis labels
const padding = { top: 40, right: 20, bottom: 70, left: yAxisWidth };
// Y axis at x=50px
```

**After:**
```javascript
const chartLeft = labelColumnWidth;  // Y axis at 150px (label column boundary)
const padding = { top: 40, right: 20, bottom: 70, left: chartLeft };
// Y axis at x=150px - exactly matches table's label column width
```

**Perfect Alignment:**
```
Table Structure:
| Label Column (150px) | Date 1 (120px) | Date 2 (120px) | Date 3 (120px) |
| "NAV Price"          | 01/01/2025     | 02/01/2025     | 03/01/2025     |

Chart Structure (Now Aligned):
| Y Axis Labels (150px)|   [Bars]       |    [Bars]      |    [Bars]      |
| $50.00               |  Centered at   |   Centered at   |   Centered at   |
| $45.00               |  x=210px       |   x=330px       |   x=450px       |
| $40.00               | (150+60)       |   (150+180)     |   (150+300)     |
                      ↑                ↑                ↑
                 Y axis line       Bar Group 1     Bar Group 2
                 at x=150px       in 120px col    in 120px col
```

**Bar Group Calculation:**
```javascript
const indexToGroupX = (index) => {
    return chartLeft + (index * dateColumnWidth) + (dateColumnWidth / 2);
};

// Bar Group 0: 150 + (0 × 120) + 60 = 210px (center of first 120px column)
// Bar Group 1: 150 + (1 × 120) + 60 = 330px (center of second 120px column)
// Bar Group 2: 150 + (2 × 120) + 60 = 450px (center of third 120px column)
```

**Y Axis Labels:**
- Positioned to the left of Y axis (at x=140px, 10px from axis)
- All within the 150px label column space
- Right-aligned (`text-anchor='end'`)

**Grid Lines:**
- Start at x=150px (Y axis position)
- Extend to right edge of chart
- Only appear in date column area (not in label column)

**Visual Result:**
- Y axis line exactly matches the table's label/date column boundary
- Bar groups perfectly centered within each date column
- Date labels align with date column headers
- Visual harmony between chart and table

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - Visual Alignment Guides for Chart (2026-01-31)

### Change: Add Visual Alignment Guides Connecting Chart to Table

**Request:** "Customize the bar svg - aligning bars with a table below the chart"

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Added Visual Alignment Elements:**

1. **Center Alignment Guide Lines:**
   ```javascript
   // Draw vertical alignment guide lines for each bar group
   paginatedNavs.forEach((nav, index) => {
       const groupCenter = indexToGroupX(index);

       const guideLine = document.createElementNS(ns, 'line');
       guideLine.setAttribute('x1', groupCenter);
       guideLine.setAttribute('y1', padding.top);
       guideLine.setAttribute('x2', groupCenter);
       guideLine.setAttribute('y2', height - padding.bottom);
       guideLine.setAttribute('stroke', '#3b82f6');
       guideLine.setAttribute('stroke-width', '1');
       guideLine.setAttribute('stroke-dasharray', '3,3');
       guideLine.setAttribute('opacity', '0.15');
   });
   ```
   - Blue dashed lines from bar group center to X axis
   - Very subtle (15% opacity)
   - Creates visual connection between bars and table columns below

2. **Column Boundary Markers:**
   ```javascript
   // Draw column boundary markers (vertical lines between date columns)
   paginatedNavs.forEach((nav, index) => {
       const colRightEdge = chartLeft + ((index + 1) * dateColumnWidth);

       const colBoundary = document.createElementNS(ns, 'line');
       colBoundary.setAttribute('x1', colRightEdge);
       colBoundary.setAttribute('y1', padding.top);
       colBoundary.setAttribute('x2', colRightEdge);
       colBoundary.setAttribute('y2', height - padding.bottom);
       colBoundary.setAttribute('stroke', '#e9ecef');
       colBoundary.setAttribute('stroke-width', '1');
       colBoundary.setAttribute('stroke-dasharray', '2,2');
   });
   ```
   - Faint gray lines at column boundaries
   - Separates each date column visually
   - Matches table's vertical structure

**Visual Guide Lines:**
```
Table:    | Label | Date 1    | Date 2    | Date 3    |
          |       | 01/01/25  | 02/01/25  | 03/01/25  |

Chart:    | $50   |     |     |     |     |
          | $45   |     |     |     |     |
          | $40   |     |     |     |     |
          |       ↓     ↓     ↓     ↓
          |      bar   bar   bar   bar
          |      grp   grp   grp   grp

Legend:   |  = Center alignment guide (blue, 15% opacity)
          |  = Column boundary (gray, very faint)
```

**Color Scheme:**
- Center guides: `#3b82f6` (blue) at 15% opacity
- Column boundaries: `#e9ecef` (light gray) at 100% opacity
- Y axis divider: `#dee2e6` (medium gray) at 100% opacity

**Benefits:**
1. **Visual Flow:** Eye can follow the center line from bar to table column
2. **Column Separation:** Clear boundaries between date columns
3. **Subtle Design:** Guides don't distract from the data
4. **Table Connection:** Creates visual link between chart and table

**Styling Details:**
- Center guides: `stroke-dasharray='3,3'` (medium dash)
- Column boundaries: `stroke-dasharray='2,2'` (small dash)
- All guides span from top padding to X axis line

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - Fixed Bar Positions Based on Table Columns (2026-01-31)

### Change: Position Each Bar Based on Column Position

**Request:** "each bar position should be based on the column position in table"

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Key Change - Fixed Bar Positions:**

**Before (centered group approach):**
```javascript
const barWidth = 40;
const barGap = 8;
const indexToGroupX = (index) => {
    return chartLeft + (index * dateColumnWidth) + (dateColumnWidth / 2);
};
// Bars positioned relative to group center
navBar.setAttribute('x', groupX - barWidth - (barGap / 2));
marketBar.setAttribute('x', groupX + (barGap / 2));
```

**After (fixed position approach):**
```javascript
const barWidth = 40;
const navBarX = (columnIndex) => chartLeft + (columnIndex * dateColumnWidth) + 15;
const marketBarX = (columnIndex) => chartLeft + (columnIndex * dateColumnWidth) + 65;
// Bars positioned at fixed locations within each column
navBar.setAttribute('x', navBarX(index));
marketBar.setAttribute('x', marketBarX(index));
```

**Position Calculation:**
- NAV bars always at `chartLeft + (columnIndex * 120) + 15`
  - Column 0: 150 + 0 + 15 = 165px
  - Column 1: 150 + 120 + 15 = 285px
  - Column 2: 150 + 240 + 15 = 405px

- Market bars always at `chartLeft + (columnIndex * 120) + 65`
  - Column 0: 150 + 0 + 65 = 215px
  - Column 1: 150 + 120 + 65 = 335px
  - Column 2: 150 + 240 + 65 = 455px

**Visual Layout:**
```
Column Layout (120px each):
|-------------------|-------------------|-------------------|
Label           Date 1           Date 2           Date 3
(150px)          (120px)          (120px)          (120px)

Bar Positioning:
|  [NAV] [Mkt]   |  [NAV] [Mkt]   |  [NAV] [Mkt]   |
 ↑150           ↑270           ↑390
  NAV=165        NAV=285        NAV=405
  Mkt=215        Mkt=335        Mkt=455
```

**Benefits:**
1. **Consistent Position:** Each bar type is at the same relative position within each column
2. **Vertical Alignment:** NAV bars across all columns are visually aligned
3. **Clear Separation:** 10px gap between NAV and Market bars (40px width each)
4. **Column Structure:** Bars clearly belong to their respective date columns

**Column Structure:**
```
Each 120px column contains:
├─ 15px padding
├─ 40px NAV bar (blue)
├─ 10px gap
├─ 40px Market bar (green)
└─ 15px padding
Total: 120px
```

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - Full Screen Width Chart (2026-01-31)

### Change: Align Bar Chart to Full Screen Width

**Request:** "barchart with should algn to the full screen width"

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Key Changes:**

1. **Fixed Wide viewBox:**
   ```javascript
   // Full screen width - use fixed wide viewBox that scales
   const totalWidth = 1200;  // Fixed viewBox width for full screen
   svg.setAttribute('viewBox', `0 0 ${totalWidth} ${height}`);
   ```

2. **Dynamic Column Width Calculation:**
   ```javascript
   // Calculate dynamic column width based on available space
   const availableWidth = totalWidth - labelColumnWidth - 20;
   const dateColumnWidth = availableWidth / paginatedNavs.length;
   ```
   - Columns now distribute evenly across full width
   - No longer fixed at 120px per column

3. **Scaled Bar Dimensions:**
   ```javascript
   const barWidth = Math.min(50, dateColumnWidth * 0.35);  // Max 50px, scales with column
   const barGap = Math.min(12, dateColumnWidth * 0.08);     // Max 12px, scales with column
   ```

4. **Centered Bar Positioning:**
   ```javascript
   const navBarX = (columnIndex) => {
       const columnCenter = chartLeft + (columnIndex * dateColumnWidth) + (dateColumnWidth / 2);
       return columnCenter - barWidth - (barGap / 2);
   };
   const marketBarX = (columnIndex) => {
       const columnCenter = chartLeft + (columnIndex * dateColumnWidth) + (dateColumnWidth / 2);
       return columnCenter + (barGap / 2);
   };
   ```

**Column Width Examples:**
```
For 1200px total width (label column = 150px):

3 dates:  (1200 - 150 - 20) / 3 = 343px per column
5 dates:  (1200 - 150 - 20) / 5 = 206px per column
10 dates: (1200 - 150 - 20) / 10 = 103px per column
```

**Responsive Bar Sizing:**
```
Wider columns (3 dates):
├─────────────────────────────────├
│  [NAV bar]  [Market bar]        │  ← 50px bars
│   50px        50px              │

Narrow columns (10 dates):
├──────────├
│ [NAV][Mkt]│  ← 36px bars (35% of 103px)
│ 36px 36px │
```

**Benefits:**
1. **Full Width Utilization:** Chart uses entire screen width
2. **Responsive Design:** Columns and bars scale based on number of dates
3. **Better Visibility:** More space for bars when showing fewer dates
4. **Consistent Spacing:** Even distribution across available width

**SVG Scaling:**
- `viewBox="0 0 1200 350"` - Fixed viewBox for consistent aspect ratio
- `width="100%"` - CSS stretches SVG to fill container
- `preserveAspectRatio="xMidYMid meet"` - Maintains proportions when scaling

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - True Full Screen Expansion (2026-01-31)

### Change: Expand Chart to Full Screen Width Without Centering

**Request:** "expand the bar chart groups to the full screen size, not center that chart"

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Key Changes:**

1. **Changed preserveAspectRatio:**
   ```html
   <!-- Before: -->
   <svg id="nav-chart" width="100%" height="350" preserveAspectRatio="xMidYMid meet" ...>

   <!-- After: -->
   <svg id="nav-chart" width="100%" height="350" preserveAspectRatio="none" ...>
   ```
   - Changed from `xMidYMid meet` (centers content) to `none` (stretches to fill)
   - Added `display: block` for proper sizing

2. **Dynamic Container Width:**
   ```javascript
   // Get actual container width for full-screen expansion
   const container = document.getElementById('nav-chart-container');
   const containerWidth = container.clientWidth || 1200;

   // Set viewBox to actual container width for full expansion
   const totalWidth = containerWidth;
   svg.setAttribute('viewBox', `0 0 ${totalWidth} ${height}`);
   ```
   - Uses actual container width instead of fixed 1200px
   - Chart expands to fill available space

3. **Window Resize Handler:**
   ```javascript
   // Handle window resize to adjust chart width
   let resizeTimeout;
   window.addEventListener('resize', function() {
       clearTimeout(resizeTimeout);
       resizeTimeout = setTimeout(function() {
           renderTableBody(); // Re-render to update chart width
       }, 250);
   });
   ```
   - Debounced resize handler (250ms delay)
   - Re-renders chart when window is resized
   - Maintains smooth performance during resize

**Comparison:**

**Before (centered):**
```
┌─────────────────────────────────────┐
│         Container (100%)             │
│   ┌───────────────────────────┐     │
│   │    Chart (centered)       │     │
│   │                           │     │
│   └───────────────────────────┘     │
└─────────────────────────────────────┘
```

**After (expanded):**
```
┌─────────────────────────────────────┐
│         Container (100%)             │
│   ┌───────────────────────────────┐ │
│   │    Chart (full width)        │ │
│   │                               │ │
│   └───────────────────────────────┘ │
└─────────────────────────────────────┘
```

**Benefits:**
1. **Full Width Utilization:** Chart fills entire container width
2. **No Centering Margins:** Eliminates empty space on sides
3. **Responsive:** Adjusts to actual container size
4. **Auto-resize:** Updates on window resize

**Technical Details:**
- `preserveAspectRatio="none"` - Allows SVG to stretch without maintaining aspect ratio
- `clientWidth` - Gets actual pixel width of container
- Debounced resize - Prevents excessive re-renders during drag-resize

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - Generic Chart Generator Function (2026-01-31)

### Change: Create Reusable JavaScript Function for Column-Aligned Bar Charts

**Request:** "need to wrap a js function that accep list of data, column chart with, generate this chart svg chart that aligns with the columns in table."

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**New Function: `generateColumnBarChart(options)`**

**Function Signature:**
```javascript
generateColumnBarChart({
    containerId: 'nav-chart',           // Target container ID
    data: [...],                          // Array of data objects
    series: [                            // Series definitions
        {key: 'NavPrice', label: 'NAV Price', color: '#3b82f6', format: 'currency'},
        {key: 'MarketPrice', label: 'Market Price', color: '#22c55e', format: 'currency'}
    ],
    labelColumnWidth: 150,               // Width of label column
    height: 350,                         // Chart height
    title: 'NAV Price vs Market Price',
    yAxis: {title: 'Price', labelFormatter: (v) => '$' + v.toFixed(2)},
    showGuides: true,                    // Show alignment guides
    showBoundaries: true                 // Show column boundaries
});
```

**Function Features:**

1. **Flexible Data Input:**
   - Accepts any array of data objects
   - Automatically detects date fields (`NavDate`, `Date`)
   - Handles null/undefined values gracefully

2. **Configurable Series:**
   - Multiple data series per column
   - Custom colors, labels, and formats
   - Supports: `currency`, `percent`, `number` formats

3. **Auto-Aligned with Table Columns:**
   - Bars positioned based on label column width
   - Columns distributed evenly across available width
   - Matches table structure automatically

4. **Responsive Design:**
   - Uses actual container width
   - Bars scale with column width
   - Full width expansion

5. **Built-in Features:**
   - Y axis with configurable labels
   - Grid lines
   - Vertical alignment guides
   - Column boundary markers
   - Interactive tooltips
   - Date formatting (en-GB)

**Usage Example 1: Historical NAV Chart**
```javascript
generateColumnBarChart({
    containerId: 'nav-chart',
    data: paginatedNavs,
    series: [
        {key: 'NavPrice', label: 'NAV Price', color: '#3b82f6', format: 'currency'},
        {key: 'MarketPrice', label: 'Market Price', color: '#22c55e', format: 'currency'}
    ],
    labelColumnWidth: 150,
    height: 350,
    yAxis: {
        title: 'Price (USD)',
        labelFormatter: (v) => '$' + v.toFixed(2)
    }
});
```

**Usage Example 2: Custom Data**
```javascript
generateColumnBarChart({
    containerId: 'my-chart',
    data: [
        {sales: 1000, target: 1200, month: 'Jan'},
        {sales: 1500, target: 1400, month: 'Feb'},
        {sales: 1300, target: 1600, month: 'Mar'}
    ],
    series: [
        {key: 'sales', label: 'Actual Sales', color: '#3b82f6', format: 'currency'},
        {key: 'target', label: 'Target', color: '#22c55e', format: 'currency'}
    ],
    labelColumnWidth: 120,
    height: 300,
    title: 'Sales vs Target',
    yAxis: {
        title: 'Amount ($)',
        labelFormatter: (v) => '$' + v.toLocaleString()
    },
    showGuides: false,
    showBoundaries: false
});
```

**Usage Example 3: Percentage Data**
```javascript
generateColumnBarChart({
    containerId: 'performance-chart',
    data: [
        {growth: 15.5, benchmark: 10.0, quarter: 'Q1'},
        {growth: 22.3, benchmark: 12.0, quarter: 'Q2'}
    ],
    series: [
        {key: 'growth', label: 'Growth %', color: '#10b981', format: 'percent'},
        {key: 'benchmark', label: 'Benchmark', color: '#64748b', format: 'percent'}
    ],
    labelColumnWidth: 150,
    height: 280
});
```

**Global Accessibility:**
```javascript
// Function available globally
window.generateColumnBarChart(options);
window.setupChartTooltips(container);
```

**Function Output:**
- Creates and injects SVG into container
- Returns SVG element for further manipulation
- Automatically sets up tooltips

**Configuration Options:**

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `containerId` | string | required | ID of target container element |
| `data` | Array | [] | Array of data objects (one per column) |
| `series` | Array | [] | Series definitions [{key, label, color, format}] |
| `labelColumnWidth` | number | 150 | Width of label column in pixels |
| `height` | number | 350 | Chart height in pixels |
| `title` | string | '' | Chart title |
| `yAxis.title` | string | 'Price' | Y axis title |
| `yAxis.labelFormatter` | function | (v) => '$' + v.toFixed(2) | Custom label formatter |
| `showGuides` | boolean | true | Show vertical alignment guides |
| `showBoundaries` | boolean | true | Show column boundary markers |

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - Refactor to Use Generic Chart Generator (2026-01-31)

### Change: Refactor renderChart to Use generateColumnBarChart Function

**Request:** "use the generateColumnBarChart function to generate the svg chart, not keeping the entire logic in one piece"

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Refactored renderChart Function:**

**Before (lines 463-716, ~250 lines of inline SVG generation):**
```javascript
// Render SVG composite bar chart
function renderChart(paginatedNavs) {
    const svg = document.getElementById('nav-chart');
    const container = document.getElementById('nav-chart-container');
    if (!svg || paginatedNavs.length === 0) {
        if (svg) {
            svg.setAttribute('viewBox', '0 0 600 350');
            svg.innerHTML = '<text x="300" y="175" text-anchor="middle" fill="#6c757d">No data available</text>';
        }
        return;
    }

    // Get actual container width for full-screen expansion
    const containerWidth = container.clientWidth || 1200;
    const height = 350;

    // Table column dimensions (for reference)
    const labelColumnWidth = 150;

    // Set viewBox to actual container width for full expansion
    const totalWidth = containerWidth;
    svg.setAttribute('viewBox', `0 0 ${totalWidth} ${height}`);

    // Calculate dynamic column width based on available space
    const availableWidth = totalWidth - labelColumnWidth - 20;
    const dateColumnWidth = availableWidth / paginatedNavs.length;

    // ... 200+ lines of SVG generation logic ...
}
```

**After (lines 463-491, ~30 lines - clean and simple):**
```javascript
// Render SVG composite bar chart - uses generic chart generator
function renderChart(paginatedNavs) {
    const svg = document.getElementById('nav-chart');
    if (!svg) return;

    if (paginatedNavs.length === 0) {
        svg.setAttribute('viewBox', '0 0 600 350');
        svg.innerHTML = '<text x="300" y="175" text-anchor="middle" fill="#6c757d">No data available</text>';
        return;
    }

    generateColumnBarChart({
        containerId: 'nav-chart',
        data: paginatedNavs,
        series: [
            { key: 'NavPrice', label: 'NAV Price', color: '#3b82f6', format: 'currency' },
            { key: 'MarketPrice', label: 'Market Price', color: '#22c55e', format: 'currency' }
        ],
        labelColumnWidth: 150,
        height: 350,
        title: 'NAV Price vs Market Price',
        yAxis: {
            title: 'Price (USD)',
            labelFormatter: (v) => '$' + v.toFixed(2)
        },
        showGuides: true,
        showBoundaries: true
    });
}
```

**Removed Code:**
- **Inline `setupChartTooltips` function (lines 689-716)** - No longer needed as the global version is used
- **~200 lines of SVG generation logic** - Now handled by `generateColumnBarChart`

**Code Reduction:**
- **Before:** ~250 lines for `renderChart` + `setupChartTooltips`
- **After:** ~30 lines for `renderChart` (calls generic function)
- **Savings:** ~220 lines of code eliminated

**Benefits:**

1. **Code Reusability:** Chart generation logic is now in a single, reusable function
2. **Maintainability:** Changes to chart behavior only need to be made in one place
3. **Readability:** `renderChart` is now clean and self-documenting
4. **Consistency:** All charts using `generateColumnBarChart` will have the same behavior
5. **DRY Principle:** Don't Repeat Yourself - chart logic defined once, used everywhere

**Function Flow:**
```
renderChart(paginatedNavs)
    ↓
generateColumnBarChart(options)
    ↓
Creates SVG with bars, axes, tooltips, guides
    ↓
Returns SVG element
```

**Configuration Object:**
The `renderChart` function now passes a clean configuration object to `generateColumnBarChart`:
- `containerId`: Target SVG element
- `data`: The historical NAV data
- `series`: Bar configurations (NAV Price in blue, Market Price in green)
- `labelColumnWidth`: 150px (matches table)
- `height`: 350px
- `title`: Chart title
- `yAxis`: Y axis configuration with title and label formatter
- `showGuides`: true (blue alignment lines)
- `showBoundaries`: true (column boundary markers)

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - Fixed Tooltip Not Showing (2026-01-31)

### Change: Fix Tooltip Persistence Across Chart Re-renders

**Request:** "nav history table, svg chart, why the tooltip is not showing now"

**Root Cause Analysis:**
The tooltip wasn't showing because of a lifecycle issue:

1. **Line 44**: Static `<div id="chart-tooltip"></div>` existed in HTML
2. **Line 362-363**: `generateColumnBarChart` does `container.innerHTML = ''` which **removes** the static tooltip
3. **Line 375-379**: `setupChartTooltips` created a new tooltip with **random ID** each time
4. **Re-render issue**: On pagination, sort, or window resize, the tooltip would be destroyed

**Problem Flow:**
```
Initial Load → Tooltip created with ID "chart-tooltip-abc123"
Re-render → container.innerHTML = '' → Tooltip destroyed
New render → New tooltip with ID "chart-tooltip-xyz789"
Event listeners attached to old bars (now gone) → No tooltip shows
```

**Implementation:**

**File Modified:** `Views/Shared/Components/HistoricalNavTable/Default.cshtml`

**Lines 375-411 - Fixed setupChartTooltips Function:**

**Before:**
```javascript
function setupChartTooltips(container) {
    const tooltip = document.createElement('div');
    tooltip.id = 'chart-tooltip-' + Math.random().toString(36).substr(2, 9);
    tooltip.style.cssText = '...';
    container.appendChild(tooltip);

    const bars = container.querySelectorAll('.chart-bar');
    bars.forEach(bar => {
        // Event listeners...
    });
}
```

**After:**
```javascript
function setupChartTooltips(container) {
    // Find or create tooltip element
    let tooltip = document.getElementById('chart-tooltip');
    if (!tooltip) {
        tooltip = document.createElement('div');
        tooltip.id = 'chart-tooltip';
        tooltip.style.cssText = '...';
        // Append to parent container (not the chart container) so it survives re-renders
        if (container.parentElement) {
            container.parentElement.appendChild(tooltip);
        } else {
            container.appendChild(tooltip);
        }
    }

    const bars = container.querySelectorAll('.chart-bar');
    bars.forEach(bar => {
        // Event listeners...
    });
}
```

**Key Changes:**

1. **Reuse Existing Tooltip:**
   ```javascript
   let tooltip = document.getElementById('chart-tooltip');
   if (!tooltip) {
       // Only create if doesn't exist
   }
   ```

2. **Fixed ID:** Uses `'chart-tooltip'` instead of random ID

3. **Parent Container Append:** Tooltip appended to `container.parentElement` instead of `container`
   - Chart container gets cleared on re-render
   - Parent container persists across re-renders
   - Tooltip survives pagination, sort, resize events

**Benefits:**

1. **Persistent Tooltip:** Single tooltip element reused across all renders
2. **Consistent ID:** Easy to debug and style with CSS
3. **Survives Re-renders:** Tooltip no longer destroyed when chart updates
4. **Event Listeners Work:** Bars always have valid tooltip reference

**Testing:**
- Hover over bar groups → Tooltip shows with value
- Change page size → Tooltip still works
- Toggle sort → Tooltip still works
- Resize window → Tooltip still works

**Build Status:** Succeeded with 0 errors

---

## Recent Updates - Reduce PDF Empty Space After Header (2026-01-31)

### Change: Reduce Empty Space Between Header and Table in PDF Export

**Request:** "for the pdf export remove some empty space between header and main table. But don't make the multi-line headers overlap with the table."

**Root Cause Analysis:**
The PDF had excessive empty space between the header and table because:
1. **Fixed spacing**: Table position was calculated as `MarginTop (100) + 30 = 130 units`
2. **Header ended at ~50-70 units** (depending on number of title/subtitle lines)
3. **Result**: 60-80 units of unnecessary empty space

**Implementation:**

**File Modified:** `Services/PdfExportService.cs`

**Lines 287-291 - Dynamic Table Position Calculation:**

**Before:**
```csharp
// Draw first page header and table header (with extra spacing after header)
DrawPageHeader(page, gfx, currentPageNum, options, font, fontHeader);
var firstPageTableTop = options.MarginTop + 30; // Extra 30 units spacing for first page
DrawTableHeader(gfx, firstPageTableTop, columnWidths, columns, fontBold, options);
```

**After:**
```csharp
// Draw first page header and calculate where table should start
double headerHeight = DrawPageHeader(page, gfx, currentPageNum, options, font, fontHeader);
// Calculate table position: header end + minimal spacing (15 units)
var firstPageTableTop = headerHeight + 15;
DrawTableHeader(gfx, firstPageTableTop, columnWidths, columns, fontBold, options);
```

**Lines 483-517 - Modified DrawPageHeader to Return Height:**

**Before:**
```csharp
/// <summary>
/// Draw first page header (report title and subtitle)
/// </summary>
private void DrawPageHeader(...)
{
    // Only first page gets full header
    if (pageNum != 1)
        return;

    // Draw header...
}
```

**After:**
```csharp
/// <summary>
/// Draw first page header (report title and subtitle)
/// Returns the Y position after the header (header bottom edge)
/// </summary>
private double DrawPageHeader(...)
{
    // Only first page gets full header
    if (pageNum != 1)
        return 20; // Return minimal position for non-first pages

    // Draw header...
    return yPos; // Return the Y position after all header content
}
```

**Key Changes:**

1. **Dynamic Header Height Calculation:**
   - `DrawPageHeader` now returns the actual Y position after drawing all header content
   - Automatically accounts for multi-line titles and subtitles

2. **Minimal Fixed Spacing:**
   - Table now starts at `headerHeight + 15` instead of `MarginTop + 30`
   - 15 units of spacing provides breathing room without excessive whitespace

3. **Multi-line Header Safe:**
   - Whether header has 1, 2, or 3 lines, table position adjusts automatically
   - No risk of header overlapping with table

**Spacing Comparison:**

| Header Lines | Before (Fixed) | After (Dynamic) | Space Saved |
|--------------|----------------|-----------------|-------------|
| 1 title line | 130 - 50 = 80 | 50 + 15 = 65 | ~50 units |
| 2 title lines | 130 - 70 = 60 | 70 + 15 = 85 | ~30 units |
| With subtitle | 130 - 95 = 35 | 95 + 15 = 110 | ~20 units |

**Benefits:**

1. **Compact Layout**: Reduces unnecessary white space in PDF exports
2. **Automatic Adjustment**: Works with any number of header lines
3. **No Overlap**: Multi-line headers won't overlap with table
4. **Consistent**: Always maintains 15 units spacing between header and table

**Testing:**
- Single-line title → Table starts closer to header
- Multi-line title with `\n` → Table adjusts automatically
- Title + subtitle → Proper spacing maintained
- No overlap regardless of header height

**Build Status:** Succeeded with 0 errors

---

**End of Documentation**
