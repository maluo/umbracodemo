# Fix Delivery API Service and Create Funds ViewComponent - 2026-02-16

## Task Checklist
- [x] Update IFundsJsonService interface to add string guid parameter
- [x] Fix GetFundsFromDeliveryApiAsync in FundsJsonService.cs
- [x] Create DeliveryApiFundsViewComponent class
- [x] Create DeliveryApiFunds/Default.cshtml view
- [x] Update HomePage.cshtml to invoke new component
- [x] Build and verify the implementation

## Implementation Details

### Problem Statement
The `GetFundsFromDeliveryApiAsync` service had critical issues:
1. **Interface/Implementation Mismatch**: Interface defined no parameters, but implementation accepted `string guid`
2. **Hardcoded GUID**: Method parameter was ignored and a hardcoded GUID was used (line 192)
3. **Inconsistent Configuration**: Different API URL patterns and header names across methods
4. **No Display Component**: Funds from Delivery API could not be displayed on the site

### Solution Implemented

#### 1. Service Layer Fixes
**File**: `Umbraco13/Services/IFundsJsonService.cs`
- Added `string guid` parameter to interface method signature

**File**: `Umbraco13/Services/FundsJsonService.cs`
- Removed hardcoded GUID that was overwriting the parameter
- Added parameter validation (null/empty check)
- Standardized API configuration to match `GetFundsByIdFromDeliveryApiAsync`:
  - Changed to use `ApiKeyAuthentication:ApiKey` config key
  - Updated header to `X-API-Key` (consistent casing)
  - Updated default URL pattern
- Enhanced logging with GUID validation warnings

#### 2. ViewComponent Implementation
**New File**: `Umbraco13/ViewComponents/DeliveryApiFundsViewComponent.cs`
- Created async ViewComponent with dependency injection
- Validates GUID parameter before service call
- Implements error handling with graceful degradation (returns empty list on failure)
- Logs errors and warnings appropriately

**New File**: `Umbraco13/Views/Shared/Components/DeliveryApiFunds/Default.cshtml`
- Created Bootstrap-styled table view
- Displays fund properties: Fund Name, Ticker Code, NAV Price, Market Price, Hold In Trust
- Shows total fund count
- Displays warning message when no data is available

#### 3. HomePage Integration
**File**: `Umbraco13/Views/HomePage.cshtml`
- Added component invocation at top of page (after header)
- Uses GUID: `5d211010-dcf8-4ea8-9daf-b5c7f0733c20`
- Positioned before existing components (NavHistory, FundsTable)

## Change Log

### Modified Files
1. **Umbraco13/Services/IFundsJsonService.cs**
   - Added `string guid` parameter to `GetFundsFromDeliveryApiAsync` method signature

2. **Umbraco13/Services/FundsJsonService.cs**
   - Removed hardcoded GUID on line 192
   - Added parameter validation
   - Standardized API configuration (URL pattern and headers)
   - Enhanced error logging

3. **Umbraco13/Views/HomePage.cshtml**
   - Added `@await Component.InvokeAsync("DeliveryApiFunds", new { guid = "..." })`

### New Files
1. **Umbraco13/ViewComponents/DeliveryApiFundsViewComponent.cs**
   - New ViewComponent class for displaying Delivery API funds

2. **Umbraco13/Views/Shared/Components/DeliveryApiFunds/Default.cshtml**
   - View template for rendering funds table

### Build Status
✅ Build succeeded with 0 errors
⚠️ 11 warnings (pre-existing, unrelated to this work)

## Testing Recommendations
1. Start the Umbraco application
2. Navigate to the home page
3. Verify new funds table appears at top of page
4. Check that data is loaded from Delivery API
5. Verify all fund properties are displayed correctly
6. Check logs for any errors related to Delivery API calls

## Future Considerations
- Move GUID from hardcoded value to appsettings.json configuration
- Implement caching to reduce Delivery API calls on each page load
- Add retry logic for transient network failures
- Consider pagination for large fund lists
