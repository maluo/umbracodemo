# Fetch Fund Data - 2026-02-16

## Implementation Plan
- Add `GetFundsByIdFromDeliveryApiAsync(Guid id)` to `IFundsJsonService`.
- Implement `GetFundsByIdFromDeliveryApiAsync(Guid id)` in `FundsJsonService` using the specified API endpoint and key.
- Parse the JSON properties for fund data.

## Change Log
- Modified `IFundsJsonService.cs`: Added `GetFundsByIdFromDeliveryApiAsync`.
- Modified `FundsJsonService.cs`: Implemented `GetFundsByIdFromDeliveryApiAsync` with JSON parsing logic.
