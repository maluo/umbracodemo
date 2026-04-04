---
name: markdown_to_pdf
description: Converts Markdown files to PDF format using Python and pandoc.
---

# Markdown to PDF Skill

This skill converts Markdown files into professional PDF documents using Python and the pandoc package.

## Usage

When the user requests to convert a markdown file to PDF:

1. **Identify the source**: Get the markdown file path from the user
2. **Check options**: Ask about:
   - Output filename (default: same as input with .pdf extension)
   - PDF engine (default: latex, optional: wkhtmltopdf, weasyprint)
   - Table of contents (default: false)
   - Syntax highlighting (default: true)
3. **Perform conversion**: Use the conversion script
4. **Verify**: Confirm the PDF was created successfully

## Command Syntax

```bash
# Basic usage (same directory, same name)
python ~/.claude/skills/markdown_to_pdf/convert.py input.md

# Custom output
python ~/.claude/skills/markdown_to_pdf/convert.py input.md -o output.pdf

# With table of contents
python ~/.claude/skills/markdown_to_pdf/convert.py input.md --toc

# Specify PDF engine
python ~/.claude/skills/markdown_to_pdf/convert.py input.md --engine wkhtmltopdf
```

## Options

| Option | Description | Default |
|--------|-------------|---------|
| `-o, --output` | Output PDF filename | `<input>.pdf` |
| `-e, --engine` | PDF engine (weasyprint, wkhtmltopdf, pdflatex, xelatex, lualatex) | weasyprint |
| `-t, --toc` | Include table of contents | false |
| `--no-highlight` | Disable syntax highlighting | true (enabled) |
| `-s, --style` | CSS style file for HTML-based engines | None |

## Features

- **Markdown rendering**: Full CommonMark + GFM support via pandoc
- **Code syntax highlighting**: Automatic language detection
- **Table support**: Proper table formatting
- **Image embedding**: Local and remote images
- **Multiple PDF engines**: LaTeX, wkhtmltopdf, weasyprint
- **Table of contents**: Optional TOC generation
- **Cross-references**: Working internal links

## Implementation

The skill uses Python with:
- `pypandoc` - Python wrapper for pandoc
- `pandoc` - Document converter (must be installed separately)

## Requirements

- Python 3.7+ installed
- pandoc installed on system
- Python packages installed in skill directory

## Setup (First Time Only)

```bash
# Install pandoc (macOS)
brew install pandoc

# Install pandoc (Ubuntu/Debian)
sudo apt-get install pandoc

# Install Python dependencies
cd ~/.claude/skills/markdown_to_pdf
pip install -r requirements.txt
```

## Error Handling

- If pandoc is not installed: Report installation instructions
- If source file doesn't exist: Report error to user
- If markdown is malformed: Attempt conversion with warnings
- If output directory is read-only: Suggest alternative location
