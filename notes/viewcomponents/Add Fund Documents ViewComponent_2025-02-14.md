# Add Fund Documents ViewComponent - 2025-02-14

## Task Checklist
- [x] Create new feature branch for FundDocumentsViewComponent
- [x] Explore codebase structure (Views, Services, ViewComponents)
- [x] Create document models (FundDocument, FundDocumentsCollection)
- [x] Create MediaDocumentsService with IMediaService and IFileSystem
- [x] Register MediaDocumentsService in Program.cs
- [x] Create FundDocumentsViewComponent
- [x] Create Default.cshtml view with tabbed UI
- [x] Build and verify implementation

## Implementation Details

### Purpose
Implement a FundDocumentsViewComponent to display fund-related documents (Factsheets, Prospectus, Reports) in a tabbed interface. Data is retrieved from a JSON file stored in the Umbraco Media library.

### Architecture

#### Models (`Umbraco13/Models/FundDocument.cs`)
- `FundDocument`: Represents a single document with Title, Url, Date, and Description
- `FundDocumentsCollection`: Dictionary mapping fund tickers to document lists
- `FundDocumentsViewModel`: Categorized documents (Factsheets, Prospectus, Reports) with HasDocuments flag

#### Service (`Umbraco13/Services/MediaDocumentsService.cs`)
- Uses `IMediaService` to search media library recursively for "funds.json"
- Uses `IFileSystem` to read file content from media storage
- Deserializes JSON using `System.Text.Json`
- Auto-categorizes documents by URL/title patterns (factsheet, prospectus, report)
- Comprehensive logging for debugging

#### ViewComponent (`Umbraco13/ViewComponents/FundDocumentsViewComponent.cs`)
- Accepts `fundTicker` parameter
- Returns empty view model if ticker is null or no documents found
- Async operation

#### Views (`Umbraco13/Views/Shared/Components/FundDocuments/`)
- `Default.cshtml`: Bootstrap tabs with conditional rendering (only shows tabs for non-empty categories)
- `_DocumentItem.cshtml`: Reusable document item partial with PDF icon and download button

### Dependency Injection
Registered in `Program.cs`:
```csharp
builder.Services.AddScoped<Umbraco13.Services.IMediaDocumentsService, Umbraco13.Services.MediaDocumentsService>();
```

## Change Log

### New Files Created
1. `Umbraco13/Models/FundDocument.cs` - Document models
2. `Umbraco13/Services/IMediaDocumentsService.cs` - Service interface
3. `Umbraco13/Services/MediaDocumentsService.cs` - Service implementation
4. `Umbraco13/ViewComponents/FundDocumentsViewComponent.cs` - ViewComponent
5. `Umbraco13/Views/Shared/Components/FundDocuments/Default.cshtml` - Tabbed UI view
6. `Umbraco13/Views/Shared/Components/FundDocuments/_DocumentItem.cshtml` - Document item partial

### Modified Files
1. `Umbraco13/Program.cs` - Added IMediaDocumentsService registration

### Usage
```cshtml
@await Component.InvokeAsync("FundDocuments", new { fundTicker = "FUNDTICKER" })
```

### Expected JSON Format
```json
{
  "Funds": {
    "FUNDTICKER": [
      {
        "Title": "Monthly Factsheet - January 2025",
        "Url": "/media/factsheet-jan2025.pdf",
        "Date": "2025-01-31",
        "Description": "Monthly performance factsheet"
      }
    ]
  }
}
```

### Build Status
- Build succeeded with 0 errors
- Minor nullable warning on MediaDocumentsService.cs:134 (not critical)

### Git Branch
- Working branch: `feat/fund-documents-viewcomponent`
