# Migration Log: Create ASP.NET Core Web API and Migrate Umbraco APIs

**Date:** 2026-02-01
**Task:** Create ASP.NET Core Web API Project and Migrate Umbraco APIs
**Location:** `/API` folder in Umbraco demo project

---

## Overview

Created a new standalone ASP.NET Core Web API project (.NET 8.0) to migrate all API endpoints from the Umbraco FundsController to a separate API service. This allows the API to run independently while sharing the same SQLite database with the Umbraco CMS.

---

## Summary of Changes

### Project Structure Created

```
/API/
├── FundsApi.sln                          # Solution file
└── FundsApi/                             # Main Web API project
    ├── Controllers/
    │   └── FundsController.cs           # 11 API endpoints (removed update-table)
    ├── Services/
    │   ├── IFundService.cs              # Interface for fund operations
    │   ├── FundService.cs               # Fund data service
    │   ├── IFundHistoricalNavService.cs # Interface for historical NAV
    │   ├── FundHistoricalNavService.cs  # Historical NAV service
    │   ├── IDownloadTokenService.cs     # Interface for token management
    │   ├── DownloadTokenService.cs      # Token generation/validation
    │   ├── IPdfExportService.cs         # Interface for PDF exports
    │   ├── PdfExportService.cs          # PDF export implementation
    │   ├── IExcelExportService.cs       # Interface for Excel exports
    │   └── ExcelExportService.cs        # Excel export implementation
    ├── Models/
    │   ├── Fund.cs                      # Fund entity model
    │   ├── FundHistoricalNav.cs         # Historical NAV entity
    │   ├── FundHistoricalNavViewModel.cs # View model
    │   └── ExportModels/
    │       ├── PdfExportModels.cs       # PDF export models
    │       └── ExcelExportModels.cs     # Excel export models
    ├── Data/
    │   └── AppDbContext.cs              # EF Core DbContext with seed data
    ├── Middleware/
    │   └── ApiKeyAuthenticationMiddleware.cs # API key authentication
    ├── Authorization/
    │   └── ValidateDownloadTokenAttribute.cs # Token validation filter
    ├── Fonts/
    │   └── FontResolver.cs              # PDF font resolver
    ├── AppData/                          # Download tokens storage
    ├── Program.cs                        # Application configuration
    ├── appsettings.json                  # Configuration file
    └── FundsApi.csproj                   # Project file
```

---

## Files Created/Modified

### New Files Created (26 files)

| File | Lines | Description |
|------|-------|-------------|
| FundsApi.sln | 18 | Solution file |
| FundsApi/FundsApi.csproj | 19 | Project file with NuGet packages |
| FundsApi/Program.cs | 167 | Web API startup configuration |
| FundsApi/appsettings.json | 35 | Configuration (CORS, JWT, API Key, EPPlus) |
| FundsApi/Controllers/FundsController.cs | 568 | 11 API endpoints |
| FundsApi/Services/IFundService.cs | 9 | Fund service interface |
| FundsApi/Services/FundService.cs | 20 | Fund service implementation |
| FundsApi/Services/IFundHistoricalNavService.cs | 25 | Historical NAV interface |
| FundsApi/Services/FundHistoricalNavService.cs | 98 | Historical NAV service |
| FundsApi/Services/IDownloadTokenService.cs | 9 | Token service interface |
| FundsApi/Services/DownloadTokenService.cs | 244 | Token service implementation |
| FundsApi/Services/IPdfExportService.cs | 23 | PDF export interface |
| FundsApi/Services/PdfExportService.cs | 797 | PDF export implementation |
| FundsApi/Services/IExcelExportService.cs | 23 | Excel export interface |
| FundsApi/Services/ExcelExportService.cs | 481 | Excel export implementation |
| FundsApi/Models/Fund.cs | 22 | Fund entity |
| FundsApi/Models/FundHistoricalNav.cs | 51 | Historical NAV entity |
| FundsApi/Models/FundHistoricalNavViewModel.cs | 46 | View model |
| FundsApi/Models/ExportModels/PdfExportModels.cs | 162 | PDF export models |
| FundsApi/Models/ExportModels/ExcelExportModels.cs | 171 | Excel export models |
| FundsApi/Data/AppDbContext.cs | 139 | DbContext with seed data |
| FundsApi/Middleware/ApiKeyAuthenticationMiddleware.cs | 141 | API key middleware |
| FundsApi/Authorization/ValidateDownloadTokenAttribute.cs | 36 | Token validation attribute |
| FundsApi/Fonts/FontResolver.cs | 93 | PDF font resolver |

### Files Copied from Umbraco13

The following files were copied from `Umbraco13/` and updated to use `FundsApi` namespace:
- All Services (FundService, FundHistoricalNavService, DownloadTokenService, PdfExportService, ExcelExportService)
- All Models (Fund, FundHistoricalNav, FundHistoricalNavViewModel)
- AppDbContext
- ApiKeyAuthenticationMiddleware
- ValidateDownloadTokenAttribute
- FontResolver

---

## API Endpoints

### Public Endpoints (No API Key Required)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/funds/download-tokens` | Get all download tokens |
| GET | `/funds/token-version` | Get current token version |
| GET | `/funds/api-key` | Get configured API key |
| GET | `/funds/test-token` | Test token validation |
| GET | `/funds/all-tokens` | Redirect message |

### Protected Endpoints (API Key Required)

| Method | Endpoint | Token Type | Description |
|--------|----------|------------|-------------|
| GET | `/funds/exporttoexcel` | excel | EPPlus Excel export |
| GET | `/funds/exporttocsv` | csv | CSV export |
| GET | `/funds/exporttoexcel-free` | excel-free | ClosedXML Excel export |
| GET | `/funds/exporttopdf` | pdf | PDF export |
| GET | `/funds/exporttoexcel-generic` | excel-generic | Generic Excel export |
| GET | `/funds/historical-nav-pdf/{fundId}` | pdf | Historical NAV PDF |
| GET | `/funds/historical-nav-excel/{fundId}` | excel-generic | Historical NAV Excel |

### Removed Endpoint

- `GET /funds/update-table` - Umbraco-specific ViewComponent endpoint (not migrated)

---

## Configuration Details

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../../Umbraco13/funds.db"
  },
  "EPPlus": {
    "ExcelPackage": { "License": "NonCommercialPersonal:james" }
  },
  "ApiKeyAuthentication": {
    "Enabled": true,
    "ApiKey": "1234567890",
    "ExcludedPaths": [
      "/funds/download-tokens",
      "/funds/api-key",
      "/swagger"
    ]
  },
  "Jwt": {
    "Issuer": "FundsApi",
    "Audience": "FundsApi",
    "SecretKey": "your-jwt-secret-key-change-this-in-production"
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:5000;https://localhost:5001"
  },
  "DownloadTokens": {
    "ExpiryInMinutes": 30
  }
}
```

### NuGet Packages (FundsApi.csproj)

- ClosedXML (0.104.2)
- EPPlus (8.4.1)
- PdfSharp (6.0.0)
- Microsoft.AspNetCore.Authentication.JwtBearer (8.0.11)
- Microsoft.EntityFrameworkCore.Sqlite (8.0.11)
- Swashbuckle.AspNetCore (6.9.0)
- Serilog.AspNetCore (8.0.0)

---

## Middleware Pipeline Order

```
1. Exception Handler
2. CORS (AllowUmbracoFrontend policy)
3. Authentication (JWT Bearer)
4. Authorization
5. API Key Middleware
6. Map Controllers
```

---

## Authentication & Authorization

### API Key Authentication
- Header: `X-API-Key`
- Query parameter: `api_key`
- Authorization header: `Bearer <key>` or `ApiKey <key>`
- Configured key: `1234567890`

### Download Token Validation
- Tokens stored in `AppData/download-tokens.json`
- Auto-refresh every 20 minutes
- Expiry: 30 minutes
- Token types: pdf, csv, excel, excel-free, excel-generic

---

## Database Sharing

- Connection string points to: `../../Umbraco13/funds.db`
- Both projects read from the same SQLite database
- SQLite supports multiple readers, one writer
- Seed data includes 224 funds and 50 historical NAV records

---

## Export Features

### PDF Export (PdfExportService)
- Custom PdfSharp-based implementation
- Bold text support using `**text**` markup
- Multi-page support
- Column auto-sizing
- Average row calculation
- Custom headers/footers
- Disclaimer support

### Excel Export (ExcelExportService)
- ClosedXML-based implementation
- Rich text formatting
- Customizable font styles
- Multi-line titles/footers
- Border control
- Auto-fit columns

---

## CORS Configuration

Named policy: `AllowUmbracoFrontend`
- Allowed origins: http://localhost:5000, https://localhost:5001
- AllowAnyHeader, AllowAnyMethod, AllowCredentials

---

## How to Run

### Command Line
```bash
cd "/Users/luoma/Downloads/backup Nov 22 2025/PVE/Umbraco/umbracodemo/API"
dotnet build
dotnet run --project FundsApi/FundsApi.csproj
```

### Using Solution File
```bash
cd "/Users/luoma/Downloads/backup Nov 22 2025/PVE/Umbraco/umbracodemo/API"
dotnet build FundsApi.sln
dotnet run --project FundsApi/FundsApi.csproj
```

### Access Swagger
Open browser and navigate to: `https://localhost:7xxx/swagger`

---

## Namespace Changes

All files copied from `Umbraco13` were updated:
- `namespace Umbraco13` → `namespace FundsApi`
- `using Umbraco13.` → `using FundsApi.`

---

## Build Status

✅ **Build Successful** - 0 errors, 0 warnings
- Framework: .NET 8.0
- Output: `FundsApi.dll`

---

## Next Steps

1. Run the API and test endpoints via Swagger
2. Test CORS from browser console
3. Verify data sync between Umbraco and API
4. Consider enabling WAL mode for better SQLite concurrency
5. Update JWT secret key in production
6. Configure production CORS origins

---

## Rollback Plan

If issues arise:
1. Delete `/API/FundsApi` folder
2. No changes were made to Umbraco13 project
3. Can continue using existing Umbraco APIs

---

## Files Modified/Created Summary

| Category | Count |
|----------|-------|
| Controllers | 1 |
| Services (interfaces) | 5 |
| Services (implementations) | 5 |
| Models | 3 |
| Export Models | 2 |
| Data | 1 |
| Middleware | 1 |
| Authorization | 1 |
| Fonts | 1 |
| Configuration | 3 (csproj, Program.cs, appsettings.json) |
| Solution | 1 (.sln) |
| **Total** | **26 files** |

---

**End of Migration Log**
