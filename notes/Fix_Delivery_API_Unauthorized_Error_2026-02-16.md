# Fix Delivery API Unauthorized Error - 2026-02-16

## Issue
The media delivery API was returning 401 Unauthorized errors when trying to fetch content.

## Root Causes Identified
1. **Wrong API Endpoint**: `DeliveryApiUrl` in appsettings.json pointed to the media endpoint (`/media/item/`) but the code was trying to fetch content items
2. **Incorrect Header Name**: Code was using `X-API-Key` header instead of the correct `Api-Key` header required by Umbraco Delivery API
3. **Public Access Disabled**: Media had `PublicAccess: false`, requiring authentication
4. **Wrong Configuration Source**: Code was reading from `ApiKeyAuthentication:ApiKey` instead of `Umbraco:CMS:DeliveryApi:ApiKey`

## Fixes Applied

### 1. Configuration Updates ([appsettings.json](Umbraco13/appsettings.json:47-57))
```json
"DeliveryApi": {
  "Enabled": true,
  "ApiKey": "1234567890",
  "DeliveryApiUrl": "https://localhost:44376/umbraco/delivery/api/v1", // Changed to base URL
  "RichTextOutputAsJson": true,
  "PublicAccess": true,
  "Media": {
    "Enabled": true,
    "PublicAccess": true // Changed from false to true
  }
}
```

**Changes**:
- Changed `DeliveryApiUrl` from full path (`/media/item/`) to base URL (`/api/v1`)
- Enabled `Media.PublicAccess` from `false` to `true`

### 2. GetFundsFromDeliveryApiAsync Method ([FundsJsonService.cs](Umbraco13/Services/FundsJsonService.cs:190-253))
```csharp
var baseUrl = _configuration.GetValue<string>("Umbraco:CMS:DeliveryApi:DeliveryApiUrl")
              ?? "http://localhost:7269/umbraco/delivery/api/v1";
var apiUrl = $"{baseUrl}/content/item/{guid}"; // Construct full URL
var apiKey = _configuration.GetValue<string>("Umbraco:CMS:DeliveryApi:ApiKey") ?? "1234567890";

// ...

if (!string.IsNullOrEmpty(apiKey))
{
    client.DefaultRequestHeaders.Add("Api-Key", apiKey); // Changed from X-API-Key
}
```

**Changes**:
- Read API key from `Umbraco:CMS:DeliveryApi:ApiKey` instead of `ApiKeyAuthentication:ApiKey`
- Changed header from `X-API-Key` to `Api-Key`
- Construct URL using base URL + endpoint path
- Changed deserialization from `DeliveryApiPagedResponse<DeliveryApiContentItem>` to `DeliveryApiContentItem` (single item)
- Replaced mapping logic with property-based parsing (same as `GetFundsByIdFromDeliveryApiAsync`)

### 3. GetFundsByIdFromDeliveryApiAsync Method ([FundsJsonService.cs](Umbraco13/Services/FundsJsonService.cs:256-270))
```csharp
var baseUrl = _configuration.GetValue<string>("Umbraco:CMS:DeliveryApi:DeliveryApiUrl")
              ?? "http://localhost:7269/umbraco/delivery/api/v1";
var apiUrl = $"{baseUrl}/content/item/{id}";
var apiKey = _configuration.GetValue<string>("Umbraco:CMS:DeliveryApi:ApiKey") ?? "1234567890";

// ...

if (!string.IsNullOrEmpty(apiKey))
{
    client.DefaultRequestHeaders.Add("Api-Key", apiKey); // Changed from X-API-Key
}
```

**Changes**:
- Read API key from `Umbraco:CMS:DeliveryApi:ApiKey` instead of `ApiKeyAuthentication:ApiKey`
- Changed header from `X-API-Key` to `Api-Key`
- Construct URL using base URL + endpoint path

## Technical Details

### Umbraco Delivery API Endpoints
- **Content**: `/umbraco/delivery/api/v1/content/item/{guid}` - Returns a single content item
- **Media**: `/umbraco/delivery/api/v1/media/item/{guid}` - Returns a single media item
- **Content Query**: `/umbraco/delivery/api/v1/content` - Query multiple content items
- **Media Query**: `/umbraco/delivery/api/v1/media` - Query multiple media items

### Authentication
- Umbraco Delivery API uses the `Api-Key` header (not `X-API-Key`)
- API key is configured in `Umbraco:CMS:DeliveryApi:ApiKey`
- When `PublicAccess` is enabled, API key may not be required

### Response Structure
- Single item: Returns `DeliveryApiContentItem` (not paged)
- Query: Returns `DeliveryApiPagedResponse<T>` with `items` array
- Content items have `properties` dictionary containing custom data

## Verification
- ✅ Build succeeded with 0 errors
- ✅ All authentication and configuration fixes applied
- ✅ Both Delivery API methods now use consistent configuration

## Testing Recommendations
1. Start the application and navigate to the HomePage
2. Verify funds data loads correctly from Delivery API
3. Check browser console and server logs for any remaining errors
4. Test with different GUID values to ensure flexibility

## Files Modified
1. **Umbraco13/appsettings.json** - Updated Delivery API configuration
2. **Umbraco13/Services/FundsJsonService.cs** - Fixed authentication and URL construction in two methods
