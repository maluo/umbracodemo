using PdfSharp.Fonts;
using System.IO;

namespace FundsApi.Fonts;

public class FontResolver : IFontResolver
{
    public byte[]? GetFont(string faceName)
    {
        try
        {
            return File.ReadAllBytes(faceName);
        }
        catch
        {
            return null;
        }
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        // Try different font names that are commonly available
        var fontNames = new[]
        {
            familyName,
            isBold ? (isItalic ? "Arial-BoldItalic" : "Arial-Bold") : (isItalic ? "Arial-Italic" : "Arial"),
            isBold ? (isItalic ? "Helvetica-BoldOblique" : "Helvetica-Bold") : (isItalic ? "Helvetica-Oblique" : "Helvetica"),
            isBold ? (isItalic ? "DejaVuSans-BoldOblique" : "DejaVuSans-Bold") : (isItalic ? "DejaVuSans-Oblique" : "DejaVuSans"),
            isBold ? (isItalic ? "LiberationSans-BoldItalic" : "LiberationSans-Bold") : (isItalic ? "LiberationSans-Italic" : "LiberationSans"),
            isBold ? (isItalic ? "FreeSans-BoldOblique" : "FreeSans-Bold") : (isItalic ? "FreeSans-Oblique" : "FreeSans"),
            isBold ? (isItalic ? "LiberationSans-BoldItalic" : "LiberationSans-Bold") : (isItalic ? "LiberationSans-Italic" : "LiberationSans")
        };

        foreach (var fontName in fontNames)
        {
            // Try macOS system fonts first
            if (OperatingSystem.IsMacOS())
            {
                var macFontPaths = new[]
                {
                    $"/System/Library/Fonts/{fontName}.ttf",
                    $"/System/Library/Fonts/Supplemental/{fontName}.ttf",
                    $"/Library/Fonts/{fontName}.ttf",
                    $"/System/Library/Fonts/{fontName}.ttc"
                };

                foreach (var path in macFontPaths)
                {
                    if (File.Exists(path))
                    {
                        return new FontResolverInfo(path);
                    }
                }
            }

            // Try Linux system fonts
            if (OperatingSystem.IsLinux())
            {
                var linuxFontPaths = new[]
                {
                    $"/usr/share/fonts/truetype/dejavu/{fontName}.ttf",
                    $"/usr/share/fonts/truetype/liberation/{fontName}.ttf",
                    $"/usr/share/fonts/truetype/freefont/{fontName}.ttf",
                    $"/usr/share/fonts/opentype/noto/{fontName}.ttf",
                    $"/usr/share/fonts/truetype/{fontName.ToLower()}.ttf",
                    $"/usr/share/fonts/{fontName}.ttf"
                };

                foreach (var path in linuxFontPaths)
                {
                    if (File.Exists(path))
                    {
                        return new FontResolverInfo(path);
                    }
                }
            }

            // Try Windows system fonts (as fallback)
            if (OperatingSystem.IsWindows())
            {
                var windowsFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), $"{fontName}.ttf");
                if (File.Exists(windowsFontPath))
                {
                    return new FontResolverInfo(windowsFontPath);
                }
            }
        }

        // If no font found, return null
        return null;
    }
}
