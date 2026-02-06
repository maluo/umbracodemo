# Robust OCR using Windows.Media.Ocr
$ErrorActionPreference = "Stop"

try {
    # Try to load the WinRT types
    [void][Windows.Media.Ocr.OcrEngine, Windows.Media.Ocr, ContentType=WindowsRuntime]
    [void][Windows.Graphics.Imaging.BitmapDecoder, Windows.Graphics.Imaging, ContentType=WindowsRuntime]
    [void][Windows.Storage.StorageFile, Windows.Storage, ContentType=WindowsRuntime]
} catch {
    Write-Host "Failed to load WinRT types. Ensure you are on Windows 10/11."
    exit 1
}

function Get-OcrText {
    param([string]$Path)
    
    $absPath = Resolve-Path $Path
    $file = [Windows.Storage.StorageFile]::GetFileFromPathAsync($absPath).GetAwaiter().GetResult()
    $stream = $file.OpenAsync([Windows.Storage.FileAccessMode]::Read).GetAwaiter().GetResult()
    $decoder = [Windows.Graphics.Imaging.BitmapDecoder]::CreateAsync($stream).GetAwaiter().GetResult()
    $bitmap = $decoder.GetSoftwareBitmapAsync().GetAwaiter().GetResult()
    
    $engine = [Windows.Media.Ocr.OcrEngine]::TryCreateFromUserProfileLanguages()
    if ($null -eq $engine) {
        throw "OCR Engine could not be created."
    }
    
    $result = $engine.RecognizeAsync($bitmap).GetAwaiter().GetResult()
    return $result.Text
}

$image = Join-Path $PSScriptRoot "images\test.png"
$output = Join-Path $PSScriptRoot "output\test.md"

if (Test-Path $image) {
    Write-Host "Extracting text from $image..."
    $text = Get-OcrText $image
    $text | Out-File -FilePath $output -Encoding utf8
    Write-Host "Success! Results saved to $output"
} else {
    Write-Host "Image not found at $image"
}
