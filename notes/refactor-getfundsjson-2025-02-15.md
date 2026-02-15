# Task Log: Refactor GetFundsJson - 2025-02-15

## Task Name
Refactor GetFundsJson to use in-memory data

## Implementation Plan

### Problem
The `GetFundsJson` endpoint in `FundsJsonController.cs` was reading a JSON file (`funds.json`) from the Umbraco Media Library on every request. This involved:
- Recursively searching the media library for the file
- Building file paths from media properties
- Reading the file from disk
- Returning raw JSON content

The `funds.json` file contains static plan/seed data (fund information with historical NAV data) that doesn't change frequently.

### Solution
Created a new `IFundsJsonService` that:
1. Loads the `funds.json` data once at application startup
2. Returns the data as a strongly-typed `FundJsonRoot` object
3. Eliminates the need for file I/O and media library lookup on each request

## Change Log

### Files Created
1. **Umbraco13/Services/IFundsJsonService.cs**
   - Interface defining `GetFundsData()` and `GetNavHistory(string tickerCode)` methods

2. **Umbraco13/Services/FundsJsonService.cs**
   - Loads `funds.json` from `AppData/funds.json` in constructor
   - Caches data in memory
   - Provides methods to get all funds data and NAV history for specific ticker

### Files Modified
1. **Umbraco13/Program.cs**
   - Added: `builder.Services.AddSingleton<IFundsJsonService, FundsJsonService>()`

2. **Umbraco13/Controllers/FundsJsonController.cs**
   - Before: 112 lines with complex file I/O and media search logic
   - After: 33 lines using injected `IFundsJsonService`
   - Removed: `IMediaService` dependency, `FindMediaByNameRecursive`, `SearchMediaRecursively` methods

3. **Umbraco13/Services/NavHistoryService.cs**
   - Before: 240 lines with duplicate file reading logic
   - After: 37 lines delegating to `IFundsJsonService`
   - Removed: `IMediaService` dependency, all helper methods, file parsing logic

## Benefits
- **Performance**: Eliminates file I/O and media library lookup on each request
- **Simplicity**: Removes ~280 lines of duplicate/complex code
- **Type Safety**: Returns strongly-typed objects instead of raw JSON strings
- **Maintainability**: Single source of truth for funds data
- **Consistency**: Both controller and service now use the same data source

## Verification
- Build succeeded with 0 errors
- Data is loaded once at application startup from `AppData/funds.json`
- API endpoint returns proper JSON structure with funds array
