---
name: OCR Text Extraction
description: Extract text from images using native Windows OCR capabilities.
---

# OCR Text Extraction Skill

This skill allows the agent to extract text from images (PNG, JPG, BMP, etc.) using the native Windows OCR engine. It is highly reliable on Windows systems and requires .NET 8.

## Capabilities
- Extract plain text from images.
- Support for multiple languages (based on Windows user profile).
- High performance using native system APIs.

## Requirements
- **.NET 8 SDK**: Must be installed on the system.
- **Windows 10/11**: Uses the `Windows.Media.Ocr` API.

## Usage

### 1. Identify the Image
Locate the absolute path to the image you want to process.

### 2. Run the OCR Tool
The skill includes a pre-configured C# tool. You can run it using:

```powershell
dotnet run --project .agent/skills/ocr/scripts/OCREngine/OCREngine.csproj -- "C:\path\to\image.png" "C:\path\to\output.md"
```

## Implementation Details
The tool targets `net8.0-windows10.0.19041.0` to access the `Windows.Media.Ocr` namespace without needing external heavy dependencies like Tesseract or OpenCV.
