# Fix NavHistoryService DI Container Error - 2026-02-14

## Task Checklist
- [x] Diagnose IFileSystem DI container error
- [x] Remove IFileSystem dependency from NavHistoryService
- [x] Implement direct file I/O using System.IO.File
- [x] Fix namespace collision between Umbraco.Cms.Core.Models.File and System.IO.File
- [x] Update NavHistoryTestFixture to create service scopes per test
- [x] Update all integration tests to use CreateService()
- [x] Build and verify all 7 integration tests pass

## Implementation Details

### Problem
The NavHistoryService had a DI container error:
```
Unable to resolve service for type 'Umbraco.Cms.Core.IO.IFileSystem' while attempting to activate 'Umbraco13.Services.NavHistoryService'
```

### Root Cause
IFileSystem is not registered in Umbraco's DI container by default. The service was trying to inject it but couldn't resolve it.

### Solution
1. **Removed IFileSystem dependency** - Simplified NavHistoryService constructor to only use IMediaService and ILogger

2. **Direct file I/O** - Changed from using IFileSystem to System.IO.File:
   - `System.IO.File.Exists(fullPath)` for file existence checks
   - `System.IO.File.OpenRead(fullPath)` for reading files
   - Used fully qualified names to avoid namespace collision

3. **Path resolution logic** - Added logic to handle both absolute and relative paths:
   ```csharp
   if (Path.IsPathRooted(physicalPath))
   {
       var fileName = mediaItem.Name ?? "funds.json";
       fullPath = System.IO.Directory.Exists(physicalPath)
           ? Path.Combine(physicalPath, fileName)
           : physicalPath;
   }
   else
   {
       fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", physicalPath.TrimStart('/'));
   }
   ```

4. **Fixed test fixture** - Modified NavHistoryTestFixture to create new service scopes per test:
   - Changed from single NavHistoryService instance to CreateService() method
   - Each test gets a fresh service instance with proper mock state
   - Prevents NSubstitute mock state pollution between tests

## Change Log

### Files Modified

**Umbraco13/Services/NavHistoryService.cs**
- Removed IFileSystem dependency from constructor
- Changed file existence check to use `System.IO.File.Exists()`
- Changed file reading to use `System.IO.File.OpenRead()`
- Added null-safe file name handling: `mediaItem.Name ?? "funds.json"`
- Added path resolution logic for absolute vs relative paths

**Umbraco13.Integration.Tests/NavHistoryIntegrationTests.cs**
- Updated all `File.WriteAllTextAsync()` calls to `System.IO.File.WriteAllTextAsync()`
- Updated `File.WriteAllText()` calls to `System.IO.File.WriteAllText()`
- Replaced `_fixture.NavHistoryService` with `_fixture.CreateService()` in all tests
- Added async modifier to `NavHistoryService_WithNullTicker_ReturnsNull` test
- Removed path manipulation code in tests (no longer needed)

**NavHistoryTestFixture class**
- Removed `NavHistoryService` property
- Added `IServiceProvider ServiceProvider` property
- Added `CreateService()` method to create scoped service instances
- Removed service resolution from constructor

### Test Results
All 7 integration tests now pass:
- ✅ ServiceCollection_ContainsRequiredServices
- ✅ NavHistoryService_WithRealDI_LoadsJsonFromDisk
- ✅ NavHistoryService_WithNestedJson_LoadsFromDisk
- ✅ NavHistoryService_WithMissingMarketPrice_LoadsFromDisk
- ✅ NavHistoryService_WithInvalidDate_HandlesGracefully
- ✅ NavHistoryService_PreservesDataOrder
- ✅ NavHistoryService_WithNullTicker_ReturnsNull

### Technical Notes
- IFileSystem is not registered in Umbraco 13 DI container by default
- IFileSystemCreator interface doesn't exist in Umbraco 13 (earlier versions only)
- Direct file I/O with System.IO.File is the recommended approach for this use case
- Creating service scopes per test prevents mock state pollution in integration tests
