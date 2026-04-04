#!/usr/bin/env python3
"""
Markdown to PDF Converter
Converts markdown files to professionally formatted PDF documents using pandoc.

Usage:
    python convert.py input.md
    python convert.py input.md -o output.pdf --toc
"""

import argparse
import sys
from pathlib import Path
import subprocess


def check_pandoc_installed() -> bool:
    """Check if pandoc is installed on the system."""
    try:
        result = subprocess.run(
            ["pandoc", "--version"],
            capture_output=True,
            text=True,
            check=False
        )
        return result.returncode == 0
    except FileNotFoundError:
        return False


def convert_markdown_to_pdf(
    input_file: Path,
    output_file: Path = None,
    engine: str = "weasyprint",
    toc: bool = False,
    highlight: bool = True,
    style_file: Path = None
) -> Path:
    """
    Convert markdown file to PDF using pandoc.

    Args:
        input_file: Path to input markdown file
        output_file: Path to output PDF file (default: input with .pdf extension)
        engine: PDF engine to use (latex, wkhtmltopdf, weasyprint)
        toc: Include table of contents
        highlight: Enable syntax highlighting
        style_file: Custom CSS file for HTML-based engines

    Returns:
        Path to the generated PDF file
    """
    if not input_file.exists():
        raise FileNotFoundError(f"Input file not found: {input_file}")

    # Set default output filename
    if output_file is None:
        output_file = input_file.with_suffix(".pdf")

    # Ensure output directory exists
    output_file.parent.mkdir(parents=True, exist_ok=True)

    # Build pandoc command
    cmd = [
        "pandoc",
        str(input_file),
        "-o", str(output_file),
        "--standalone",
        "--pdf-engine", engine,
    ]

    # Add table of contents if requested
    if toc:
        cmd.extend(["--toc", "--toc-depth=3"])

    # Syntax highlighting
    if highlight:
        cmd.extend(["--highlight-style", "pygments"])

    # Add metadata for better PDF formatting
    metadata = [
        "mainfont=Helvetica",
        "sansfont=Helvetica",
        "monofont=Courier",
        "fontsize=11pt",
        "geometry:margin=1.5cm",
    ]

    for meta in metadata:
        cmd.extend(["-M", meta])

    # Add variable for better line breaks
    cmd.extend(["-V", "geometry:a4paper"])

    # For HTML-based engines, add CSS if provided
    if style_file and style_file.exists():
        if engine in ["wkhtmltopdf", "weasyprint"]:
            cmd.extend(["--css", str(style_file)])

    # Execute pandoc
    try:
        result = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            check=True
        )
        print(f"✓ Successfully converted '{input_file}' to '{output_file}'")
        return output_file
    except subprocess.CalledProcessError as e:
        error_msg = e.stderr or e.stdout or str(e)
        raise RuntimeError(f"Pandoc conversion failed: {error_msg}")


def parse_args(args: list[str] = None) -> argparse.Namespace:
    """Parse command line arguments."""
    parser = argparse.ArgumentParser(
        description="Convert Markdown files to PDF using pandoc",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  %(prog)s input.md
  %(prog)s input.md -o output.pdf
  %(prog)s input.md --toc --engine wkhtmltopdf
  %(prog)s input.md -s custom.css
        """
    )

    parser.add_argument(
        "input",
        type=Path,
        help="Input markdown file"
    )

    parser.add_argument(
        "-o", "--output",
        type=Path,
        default=None,
        help="Output PDF filename (default: input filename with .pdf extension)"
    )

    parser.add_argument(
        "-e", "--engine",
        choices=["weasyprint", "wkhtmltopdf", "pdflatex", "xelatex", "lualatex", "latexmk", "prince", "typst", "context"],
        default="weasyprint",
        help="PDF engine to use (default: weasyprint)"
    )

    parser.add_argument(
        "-t", "--toc",
        action="store_true",
        help="Include table of contents"
    )

    parser.add_argument(
        "--no-highlight",
        action="store_true",
        help="Disable syntax highlighting"
    )

    parser.add_argument(
        "-s", "--style",
        type=Path,
        default=None,
        help="CSS style file for HTML-based PDF engines"
    )

    return parser.parse_args(args)


def main(args: list[str] = None) -> int:
    """Main entry point."""
    parsed_args = parse_args(args)

    # Check if pandoc is installed
    if not check_pandoc_installed():
        print("Error: pandoc is not installed on your system.", file=sys.stderr)
        print("", file=sys.stderr)
        print("Installation instructions:", file=sys.stderr)
        print("  macOS:   brew install pandoc", file=sys.stderr)
        print("  Ubuntu:  sudo apt-get install pandoc", file=sys.stderr)
        print("  Windows: Download from https://pandoc.org/installing.html", file=sys.stderr)
        return 1

    try:
        convert_markdown_to_pdf(
            input_file=parsed_args.input,
            output_file=parsed_args.output,
            engine=parsed_args.engine,
            toc=parsed_args.toc,
            highlight=not parsed_args.no_highlight,
            style_file=parsed_args.style
        )
        return 0
    except FileNotFoundError as e:
        print(f"Error: {e}", file=sys.stderr)
        return 1
    except RuntimeError as e:
        print(f"Error: {e}", file=sys.stderr)
        return 1
    except Exception as e:
        print(f"Unexpected error: {e}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
