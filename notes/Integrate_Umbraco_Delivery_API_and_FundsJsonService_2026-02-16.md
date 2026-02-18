# Integrate Umbraco Delivery API and FundsJsonService - 2026-02-16

## Task Checklist
- [x] Audit Umbraco 13 Delivery API setup and security
    - [x] Investigate "IIndexPopulator" error and Delivery API configuration
    - [x] Review `ApiKeyAuthenticationMiddleware.cs` and its integration with Delivery API
- [x] Implement Delivery API configuration and security
    - [x] Update `appsettings.json` and standardized `DeliveryApi` config
    - [x] Update `ApiKeyAuthenticationMiddleware.cs` for Delivery API routes
- [x] Integrate Delivery API into `FundsJsonService`
    - [x] Update `IFundsJsonService.cs`
    - [x] Implement API call in `FundsJsonService.cs`
    - [x] Create models for Delivery API response
- [x] Log results to the `notes` folder

## Implementation Details
### Configuration
- Standardized `Umbraco:CMS:DeliveryApi` configuration in `appsettings.json`.
- Added `DeliveryApiUrl` for the service to call.
- Updated `ApiKeyAuthentication` `ExcludedPaths` to include `/umbraco/delivery`.

### Services & Models
- Created `DeliveryApiModels.cs` for API response handling.
- Added `GetFundsFromDeliveryApiAsync` to `IFundsJsonService` and implemented it in `FundsJsonService`.
- Registered `IHttpClientFactory` in `Program.cs`.

## Change Log
### Modified Files:
- [appsettings.json](file:///Users/luoma/Downloads/backup%20Nov%2022%202025/PVE/Umbraco/umbracodemo/Umbraco13/appsettings.json): Enabled Delivery API and configured routes.
- [Program.cs](file:///Users/luoma/Downloads/backup%20Nov%2022%202025/PVE/Umbraco/umbracodemo/Umbraco13/Program.cs): Registered `HttpClient`.
- [IFundsJsonService.cs](file:///Users/luoma/Downloads/backup%20Nov%2022%202025/PVE/Umbraco/umbracodemo/Umbraco13/Services/IFundsJsonService.cs): Added async method for Delivery API.
- [FundsJsonService.cs](file:///Users/luoma/Downloads/backup%20Nov%2022%202025/PVE/Umbraco/umbracodemo/Umbraco13/Services/FundsJsonService.cs): Implemented the API call and mapping logic.

### New Files:
- [DeliveryApiModels.cs](file:///Users/luoma/Downloads/backup%20Nov%2022%202025/PVE/Umbraco/umbracodemo/Umbraco13/Models/DeliveryApiModels.cs): Response models for correctly parsing Umbraco Delivery API JSON.
