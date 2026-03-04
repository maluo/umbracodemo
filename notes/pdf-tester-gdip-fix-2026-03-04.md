# PDF Tester Gdip Fix - 2026-03-04

## Task Checklist
- [x] Migrate from PdfSharp to PdfSharpCore (cross-platform)
- [x] Test PDF generation to verify the fix

## Implementation Details

### Problem
The PDF Tester application was failing with a `System.TypeInitializationException` related to `Gdip` on macOS. The root cause was that the old `PdfSharp 1.50.5147` package depends on `System.Drawing.Common`, which:
1. Only works on Windows starting from .NET 6
2. Requires native Windows libraries (user32.dll, GDI+) that don't exist on macOS

### Solution
Migrated from the legacy `PdfSharp` package to `PdfSharpCore`, a .NET Core port that is fully cross-platform and doesn't depend on System.Drawing.Common or Windows-specific libraries.

### Technical Changes

#### Package Updates (PdfTester.csproj)
**Before:**
```xml
<PackageReference Include="PdfSharp" Version="1.50.5147" />
<PackageReference Include="System.Drawing.Common" Version="8.0.0" />
```

**After:**
```xml
<PackageReference Include="PdfSharpCore" Version="1.3.65" />
```

#### Namespace Updates
Updated all using statements across three files:
- `Program.cs`: `PdfSharp.Drawing` → `PdfSharpCore.Drawing`, `PdfSharp.Pdf` → `PdfSharpCore.Pdf`
- `PdfService.cs`: `PdfSharp` → `PdfSharpCore` (including Drawing, Pdf namespaces)
- `PdfExportService.cs`: `PdfSharp` → `PdfSharpCore` (including Drawing, Pdf namespaces)

#### Code Changes
- Fixed reference: `PdfSharp.PageSize.A4` → `PageSize.A4` (namespace already imported)

### Build Results
- Build: Success (0 warnings, 0 errors)
- Runtime: All PDF generation tests passed successfully

## Change Log

### Files Modified
1. `htmlToPDF/PdfTester/PdfTester.csproj` - Replaced PdfSharp + System.Drawing.Common with PdfSharpCore
2. `htmlToPDF/PdfTester/Program.cs` - Updated using statements for PdfSharpCore namespaces
3. `htmlToPDF/PdfTester/PdfService.cs` - Updated using statements and fixed PageSize reference
4. `htmlToPDF/PdfTester/PdfExportService.cs` - Updated using statements for PdfSharpCore namespaces

### Files Removed
- `htmlToPDF/PdfTester/runtimeconfig.template.json` - No longer needed after migration

### Test Results
All 4 PDF files generated successfully:
- `output_employees.pdf` (80,861 bytes)
- `output_products.pdf` (79,281 bytes)
- `output_students.pdf` (79,183 bytes)
- `output_funds_export.pdf` (79,158 bytes)

### Additional Notes
- The `mono-libgdiplus` package was installed via Homebrew during troubleshooting, but is not required with PdfSharpCore
- This solution is cross-platform and will work on Windows, macOS, and Linux without any native library dependencies
