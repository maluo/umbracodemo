# PDF Metadata Feature - 2026-02-08

## Task Name
Add PDF metadata properties (DocumentTitle, Author, Description) to PDF export service

## Task Checklist
- [x] Create feature branch: `feat/pdf-document-title-option`
- [x] Add DocumentTitle property to PdfExportOptions (Umbraco13)
- [x] Add DocumentTitle property to PdfExportOptions (API)
- [x] Set document.Info.Title when creating PDF documents
- [x] Add Author property to PdfExportOptions (Umbraco13)
- [x] Add Author property to PdfExportOptions (API)
- [x] Add Description property to PdfExportOptions (Umbraco13)
- [x] Add Description property to PdfExportOptions (API)
- [x] Set document.Info.Author and document.Info.Subject
- [x] Build and verify Umbraco13 project (0 errors)
- [x] Build and verify API project (0 errors)
- [x] Commit changes with conventional commit format
- [x] Push feature branch to remote
- [x] Log task completion

## Implementation Details

### Overview
Added three PDF metadata properties to the PDF export service to allow users to specify document information that appears in PDF viewer properties and document metadata.

### Technical Approach

#### PDF Metadata Fields
The PDF specification supports several metadata fields via `PdfDocument.Info`:
- **Title** - Shows in PDF viewer title bar and window title
- **Author** - Document creator/author information
- **Subject** - Document description/summary (Description property maps to Subject)

#### Property Design

1. **DocumentTitle Property**
   - Type: `string?` (nullable)
   - Purpose: Sets the PDF document title metadata
   - Default behavior: If not specified, falls back to `ReportTitle`
   - Mapping: `document.Info.Title`

2. **Author Property**
   - Type: `string?` (nullable)
   - Purpose: Sets the PDF document author
   - Default: `null` (optional)
   - Mapping: `document.Info.Author`
   - Only set if not null or empty

3. **Description Property**
   - Type: `string?` (nullable)
   - Purpose: Sets the PDF document subject/description
   - Default: `null` (optional)
   - Mapping: `document.Info.Subject`
   - Only set if not null or empty

### Code Changes

#### 1. Umbraco13/Services/PdfExportService.cs

**Added properties to PdfExportOptions:**
```csharp
public string? DocumentTitle { get; set; }
public string? Author { get; set; }
public string? Description { get; set; }
```

**Updated ExportToPdf method to set metadata:**
```csharp
var document = new PdfDocument();

// Set document title metadata
document.Info.Title = !string.IsNullOrEmpty(options.DocumentTitle)
    ? options.DocumentTitle
    : options.ReportTitle;

// Set document author metadata
if (!string.IsNullOrEmpty(options.Author))
{
    document.Info.Author = options.Author;
}

// Set document description/subject metadata
if (!string.IsNullOrEmpty(options.Description))
{
    document.Info.Subject = options.Description;
}
```

#### 2. API/FundsApi/Models/ExportModels/PdfExportModels.cs

**Added same properties to API version:**
```csharp
public string? DocumentTitle { get; set; }
public string? Author { get; set; }
public string? Description { get; set; }
```

#### 3. API/FundsApi/Services/PdfExportService.cs

**Applied same metadata setting logic as Umbraco13 version**

### Design Decisions

1. **Nullable Properties**: All three properties are nullable to make them optional
2. **Fallback for Title**: `DocumentTitle` falls back to `ReportTitle` for convenience
3. **Conditional Setting**: `Author` and `Description` only set if provided (no defaults)
4. **Subject vs Description**: PDF metadata uses "Subject" field, but property is named "Description" for clarity

## Change Log

### Commits Created

1. **First Commit**: `feat: add DocumentTitle option for PDF metadata`
   - Added DocumentTitle property
   - Set document.Info.Title
   - Updated both Umbraco13 and API versions
   - Hash: d4479ae

2. **Second Commit**: `feat: add Author and Description properties for PDF metadata`
   - Added Author property
   - Added Description property
   - Set document.Info.Author and document.Info.Subject
   - Updated both Umbraco13 and API versions
   - Hash: 936b746

### Files Modified

1. **Umbraco13/Services/PdfExportService.cs**
   - Added 3 properties to PdfExportOptions class
   - Updated ExportToPdf method to set metadata

2. **API/FundsApi/Models/ExportModels/PdfExportModels.cs**
   - Added 3 properties to PdfExportOptions class

3. **API/FundsApi/Services/PdfExportService.cs**
   - Updated ExportToPdf method to set metadata

### Build Results

✅ **Umbraco13**: Build succeeded - 0 errors (10 pre-existing warnings)
✅ **API**: Build succeeded - 0 errors, 0 warnings

### Usage Examples

```csharp
// Basic usage - only title
var pdfOptions = new PdfExportOptions
{
    ReportTitle = "Annual Financial Report",
    DocumentTitle = "FY2025 Q4 Report"
};

// Complete usage - all metadata
var pdfOptions = new PdfExportOptions
{
    ReportTitle = "Annual Financial Report",        // Shows on page
    DocumentTitle = "FY2025 Q4 Report",             // PDF title (viewer title bar)
    Author = "John Doe",                             // Document author
    Description = "Quarterly financial summary including revenue, expenses, and profit analysis", // Subject/description
    // ... other options
};
```

### Git Workflow Followed

1. ✅ Created feature branch: `feat/pdf-document-title-option`
2. ✅ Made changes on feature branch (not `main`)
3. ✅ Built both projects successfully (0 errors)
4. ✅ Committed with conventional commit format
5. ✅ Pushed to remote with tracking
6. ✅ Multiple commits on same branch for related changes

### Branch Information

- **Branch Name**: `feat/pdf-document-title-option`
- **Remote**: https://github.com/maluo/umbracodemo
- **Pull Request URL**: https://github.com/maluo/umbracodemo/pull/new/feat/pdf-document-title-option
- **Status**: Ready for review and merge

### Notes

- All three metadata properties are optional
- DocumentTitle defaults to ReportTitle for backward compatibility
- Author and Description only set when explicitly provided
- PDF metadata is visible in Adobe Acrobat, browser PDF viewers, and other PDF tools
- Properties appear in File → Properties dialog in most PDF viewers
- Search engines can index PDF metadata for better SEO

### Related Features

This feature complements the existing PDF export capabilities:
- Report display options (ReportTitle, Subtitle)
- Formatting options (fonts, sizes, colors)
- Layout options (margins, page size, headers, footers)
- Border options (heading, disclaimer, footer borders)
