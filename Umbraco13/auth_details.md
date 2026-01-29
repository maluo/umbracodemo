# Authentication & Authorization System - Complete Documentation

## Overview

The application implements a dual authentication system:
1. **API Key Authentication** - For API endpoints (specifically for UpdateTable)
2. **Token-based Authentication** - For download endpoints (PDF, CSV, Excel)

---

## 1. API Key Authentication

### Configuration (`appsettings.json`)

```json
{
  "ApiKeyAuthentication": {
    "Enabled": true,
    "ApiKey": "1234567890",
    "AllowAnonymous": false,
    "ExcludedPaths": [
      "/",
      "/umbraco",
      "/funds",
      "/funds/download-tokens",
      "/funds/token-version",
      "/funds/api-key",
      "/funds/test-token",
      "/funds/all-tokens",
      "/funds/exporttopdf",
      "/funds/exporttocsv",
      "/funds/exporttoexcel-free",
      "/funds/exporttoexcel",
      "/api",
      "/media",
      "/App_Plugins",
      "/css",
      "/js",
      "/images",
      "/fonts"
    ]
  }
}
```

### Middleware: `ApiKeyAuthenticationMiddleware.cs`

**Location**: `/Middleware/ApiKeyAuthenticationMiddleware.cs`

**How it works:**

1. **Order of checks:**
   - First checks if API key authentication is enabled
   - Then checks for `[AllowAnonymous]` attribute on the endpoint
   - Then checks if path is in `ExcludedPaths`
   - If all checks fail, requires API key validation

2. **API Key Sources** (checked in order):
   - Header: `X-API-Key: 1234567890`
   - Header: `Authorization: Bearer 1234567890`
   - Header: `Authorization: ApiKey 1234567890`
   - Query parameter: `?api_key=123456890`

3. **When API key is valid:**
   - Creates claims principal with:
     - Name: "ApiKeyUser"
     - Role: "ApiUser"
   - Allows request to proceed

4. **When API key is missing/invalid:**
   - Returns 401 Unauthorized
   - JSON response: `{"error":"API key is required"}` or `{"error":"Invalid API key"}`

### Endpoint: Get API Key

**Endpoint**: `GET /funds/api-key`

**Attributes**: `[AllowAnonymous]` (excluded from API key check)

**Response**:
```json
{
  "apiKey": "1234567890"
}
```

**Usage**: Allows frontend to fetch the current API key from server configuration

---

## 2. Token-Based Authentication (Downloads)

### Token Service: `DownloadTokenService.cs`

**Location**: `/Services/DownloadTokenService.cs`

**Token Storage**: `AppData/download-tokens.json`

**File Structure**:
```json
{
  "Version": "638800000000000000",
  "Tokens": {
    "pdf": {
      "Token": "random44CharString",
      "Expiry": "2025-01-28T12:00:00Z",
      "Type": "pdf"
    },
    "csv": { ... },
    "excel-free": { ... },
    "excel": { ... }
  }
}
```

**Lifecycle**:
- **Generated**: 44-character URL-safe base64 string (random bytes, no HMAC)
- **Refresh**: Every 20 minutes (background timer)
- **Expiry**: 30 minutes after generation
- **Version**: Changes each time tokens are refreshed (using `DateTime.UtcNow.Ticks`)

### Token Endpoints

**Get Tokens**: `GET /funds/download-tokens`
- **Access**: Anonymous (in excluded paths)
- **Response**:
```json
{
  "version": "638800000000000000",
  "pdf": "token1",
  "csv": "token2",
  "excel": "token3",
  "excelEpPlus": "token4"
}
```

**Get Version**: `GET /funds/token-version`
- **Access**: Anonymous (in excluded paths)
- **Response**:
```json
{
  "version": "638800000000000000"
}
```

### Download Endpoints (Require Token, No API Key)

All endpoints in `FundsController.cs`:
- `GET /funds/exporttopdf?token=...` → PDF download
- `GET /funds/exporttocsv?token=...` → CSV download
- `GET /funds/exporttoexcel-free?token=...` → Excel (ClosedXML) download
- `GET /funds/exporttoexcel?token=...` → Excel (EPPlus) download

**Authentication**: `ValidateDownloadToken` attribute
- Reads token from query string
- Validates against `AppData/download-tokens.json`
- Checks: token matches, type matches, not expired
- Returns 401 if invalid: `{"error":"Valid download token required"}`

---

## 3. Frontend Integration

### JavaScript Token Management

**Initial Load**:
```javascript
// 1. Fetch API key for authenticated requests
fetchApiKey().then(() => {
  fetchAndUpdateTokens();
});

// 2. Get tokens
fetch('/funds/download-tokens')
  .then(r => r.json())
  .then(tokens => {
    currentTokenVersion = tokens.version;
    updateDownloadLinks(tokens);
  });
```

**Token Version Polling**:
```javascript
// Check every 30 seconds if tokens were refreshed
setInterval(checkTokenVersion, 30000);

function checkTokenVersion() {
  fetch('/funds/token-version')
    .then(r => r.json())
    .then(data => {
      if (data.version !== currentTokenVersion) {
        fetchAndUpdateTokens();
      }
    });
}
```

**Update Table with API Key**:
```javascript
function reloadFundsTable() {
  fetchApiKey().then(apiKey => {
    fetch('/funds/update-table', {
      headers: { 'X-API-Key': apiKey }
    })
      .then(response => response.text())
      .then(html => {
        document.getElementById('funds-table-container').innerHTML = html;
        fetchAndUpdateTokens();
      });
  });
}
```

---

## 4. Authentication Flow Summary

```
┌─────────────────────────────────────────────────────────────┐
│                    AUTHENTICATION FLOW                       │
└─────────────────────────────────────────────────────────────┘

HOME PAGE
   │
   └─ No auth required (in ExcludedPaths)

/funds/update-table
   │
   ├─ Middleware checks: Not in ExcludedPaths → Requires API Key
   ├─ Frontend sends: X-API-Key header
   └─ Returns: HTML table content

/funds/download-tokens, /funds/token-version, /funds/api-key
   │
   └─ In ExcludedPaths → No API Key required

/funds/exporttopdf?token=... (and other download endpoints)
   │
   ├─ In ExcludedPaths → Middleware bypassed
   ├─ ValidateDownloadToken attribute checks token
   └─ Token must match stored token in AppData/download-tokens.json
```

---

## 5. Security Notes

1. **API Key Security**:
   - Key is returned by `/funds/api-key` (anyone can call it)
   - Should be used in a trusted environment or with additional restrictions
   - Consider IP restrictions or HTTPS only in production

2. **Token Security**:
   - Tokens are stored server-side (not visible in source code)
   - Tokens expire in 30 min, refresh every 20 min
   - Tokens are validated against file, not cryptographic signature

3. **Public Access**:
   - Home page: Public
   - Download endpoints: Requires valid token
   - Update table: Requires API key
   - Token endpoints: Public (for token distribution)

---

## 6. Configuration Reference

### Program.cs Middleware Registration
```csharp
// API Key middleware must be BEFORE Umbraco middleware
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
```

### Service Registration
```csharp
services.AddScoped<IDownloadTokenService, DownloadTokenService>();
```

---

## 7. Troubleshooting

**401 Unauthorized on UpdateTable:**
- Check API key in `X-API-Key` header matches `appsettings.json`
- Ensure `/funds/update-table` is NOT in `ExcludedPaths`

**Token validation fails:**
- Check token version matches server version
- Verify `AppData/download-tokens.json` exists and has valid format
- Check logs for validation errors

**API key returns empty:**
- Check `appsettings.json` has valid API key configured
- Check middleware is returning value correctly

---

## File Locations

- `Middleware/ApiKeyAuthenticationMiddleware.cs` - API key validation
- Services/DownloadTokenService.cs - Token generation/validation
- Services/IDownloadTokenService.cs - Service interface
- Authorization/ValidateDownloadTokenAttribute.cs - Token validation attribute
- Controllers/FundsController.cs - Endpoints
- Views/Shared/Components/FundsTable/Default.cshtml - Frontend
- AppData/download-tokens.json - Token storage
