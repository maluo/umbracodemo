# Authentication System Documentation

## Overview
This document describes the authentication and authorization system for the Umbraco13 Funds application.

## API Key Authentication

### Configuration
The API key is configured in `appsettings.json`:
```json
{
  "ApiKeyAuthentication": {
    "Enabled": true,
    "ApiKey": "1234567890",
    "AllowAnonymous": false,
    "ExcludedPaths": [...]
  }
}
```

### Middleware
The `ApiKeyAuthenticationMiddleware` handles API key validation:
- Checks for API key in: `X-API-Key` header, `Authorization: Bearer` header, or `api_key` query parameter
- Validates against the configured API key
- Skips authentication for paths listed in `ExcludedPaths`
- Creates authenticated user with claims when API key is valid

### API Key Endpoint
- **Endpoint**: `GET /funds/api-key`
- **Access**: Anonymous (in excluded paths)
- **Returns**: Current API key from configuration
```json
{"apiKey":"1234567890"}
```

### Frontend Usage
The frontend fetches the API key dynamically:
```javascript
fetch('/funds/api-key')
  .then(r => r.json())
  .then(data => apiKey = data.apiKey)
```

Then uses it for authenticated requests:
```javascript
fetch('/funds/update-table', {
  headers: { 'X-API-Key': apiKey }
})
```

## Token Authentication (for Downloads)

### Download Token Service
Tokens stored in `AppData/download-tokens.json`:
```json
{
  "Version": "timestamp",
  "Tokens": {
    "pdf": { "Token": "...", "Expiry": "...", "Type": "pdf" },
    "csv": { "Token": "...", "Expiry": "...", "Type": "csv" },
    "excel-free": { "Token": "...", "Expiry": "...", "Type": "excel-free" },
    "excel": { "Token": "...", "Expiry": "...", "Type": "excel" }
  }
}
```

### Token Lifecycle
- Generated at startup if file doesn't exist
- Refreshed every 20 minutes
- Tokens expire after 30 minutes
- Frontend polls for version changes every 30 seconds

### Token Endpoints
- `GET /funds/download-tokens` - Get current tokens with version
- `GET /funds/token-version` - Check if tokens have been refreshed

### Download Endpoints
Require valid token (no API key needed):
- `GET /funds/exporttopdf?token=...`
- `GET /funds/exporttocsv?token=...`
- `GET /funds/exporttoexcel-free?token=...`
- `GET /funds/exporttoexcel?token=...`

## Security Considerations
1. `/funds/api-key` is publicly accessible but only returns the configured API key
2. Download endpoints use token-based auth (tokens stored server-side)
3. Update table endpoint requires API key authentication
4. Home page and Umbraco backoffice are excluded from API key requirement
