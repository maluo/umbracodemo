using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;

namespace OCREngine
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: OCREngine <image-path> [output-path]");
                return;
            }

            string imagePath = Path.GetFullPath(args[0]);
            string outputPath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.ChangeExtension(imagePath, ".md");

            try
            {
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine($"Error: File not found {imagePath}");
                    return;
                }

                StorageFile file = await StorageFile.GetFileFromPathAsync(imagePath);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync();

                    OcrEngine engine = OcrEngine.TryCreateFromUserProfileLanguages();
                    if (engine == null)
                    {
                        Console.WriteLine("Error: Could not create OCR Engine.");
                        return;
                    }

                    OcrResult result = await engine.RecognizeAsync(bitmap);
                    
                    // Simple formatting: join lines
                    string text = result.Text;

                    await File.WriteAllTextAsync(outputPath, text);
                    Console.WriteLine($"Successfully extracted text to {outputPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
