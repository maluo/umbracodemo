# Task: Fix Google Authenticator package

## Implementation Plan

1. **Identify the root cause**: Compile errors in the project showed that the namespace `Google` could not be found, indicating that the `GoogleAuthenticator` NuGet package reference was missing. Git history revealed that it had been removed during a previous commit.
2. **Restore Package Reference**: Add `<PackageReference Include="GoogleAuthenticator" Version="3.2.0" />` back to `Umbraco13/Umbraco13.csproj`.
3. **Resolve Secondary Compile Issues**:
   - Remove registration of `IHtmlToPdfConverter` and `PdfSharpHtmlConverter` in `Umbraco13/Program.cs` since these types were part of an incomplete commit and do not exist.
   - Replace `XFontStyleEx` (which only exists in `PdfSharpCore`) with standard `XFontStyle` in `Umbraco13/Services/PdfExportService.cs`.
4. **Verify**: Build the solution and verify that the compilation succeeds.

## Change Log

### Umbraco13

#### [Umbraco13.csproj](file:///Users/luoma/Downloads/backup%20Nov%2022%202025/PVE/Umbraco/umbracodemo/Umbraco13/Umbraco13.csproj)
- Added back the NuGet PackageReference `GoogleAuthenticator` version `3.2.0`.

#### [Program.cs](file:///Users/luoma/Downloads/backup%20Nov%2022%202025/PVE/Umbraco/umbracodemo/Umbraco13/Program.cs)
- Removed registration of non-existent `IHtmlToPdfConverter` and `PdfSharpHtmlConverter` dependencies:
  ```diff
  -builder.Services.AddScoped<Umbraco13.Services.IHtmlToPdfConverter, Umbraco13.Services.PdfSharpHtmlConverter>();
  ```

#### [PdfExportService.cs](file:///Users/luoma/Downloads/backup%20Nov%2022%202025/PVE/Umbraco/umbracodemo/Umbraco13/Services/PdfExportService.cs)
- Replaced `XFontStyleEx` with `XFontStyle` to match the standard PdfSharp library signature:
  ```diff
  -var fontBold = new XFont(options.FontFamily, options.HeaderFontSize, XFontStyleEx.Bold);
  +var fontBold = new XFont(options.FontFamily, options.HeaderFontSize, XFontStyle.Bold);
  ```
