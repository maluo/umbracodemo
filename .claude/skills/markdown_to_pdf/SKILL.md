# Markdown to PDF Conversion Skill

This skill converts markdown files to PDF format using `markdown-pdf` npm package.

## Capabilities
- Convert markdown files (.md) to PDF
- Support for markdown tables with grid lines
- Preserve formatting, headers, and tables

## Requirements
- **Node.js**: v18 or higher
- **Dependencies**: `markdown-pdf` (installed as dev dependency)

## Installation

The `markdown-pdf` package is already installed in the project. If needed:

```bash
npm install --save-dev markdown-pdf
```

## Usage

### Convert a Single File

```bash
node -e "
const markdownPdf = require('markdown-pdf');
const mdPath = 'path/to/your/file.md';
const pdfPath = 'path/to/your/file.pdf';

markdownPdf().from(mdPath).to(pdfPath, function() {
  console.log('PDF created:', pdfPath);
});
"
```

### Using the Skill

When you need to convert a markdown file to PDF:

1. Provide the path to the markdown file
2. The PDF will be generated in the same directory with the same filename but .pdf extension

## Markdown Table Grid Format

For tables with grid lines in the PDF, use GitHub-style markdown tables:

```markdown
| Header 1 | Header 2 | Header 3 |
|:---|:---:|---:|
| Left | Center | Right |
| Data | Data | Data |
```

- `|:---|` - Left aligned
- `|:---:|` - Center aligned
- `|---:|` - Right aligned

## Examples

### Simple Conversion

```bash
node -e "
const markdownPdf = require('markdown-pdf');
markdownPdf().from('notes/invoice.md').to('notes/invoice.pdf', () => {});
"
```

### With Custom Options

```bash
node -e "
const markdownPdf = require('markdown-pdf');
markdownPdf({
  remarkable: {
    html: true,
    breaks: true
  }
}).from('input.md').to('output.pdf', () => {});
"
```

## Implementation Details

- Uses `markdown-pdf` which wraps `phantomjs` for PDF generation
- Supports standard markdown syntax
- Tables are rendered with borders when using proper markdown table syntax
