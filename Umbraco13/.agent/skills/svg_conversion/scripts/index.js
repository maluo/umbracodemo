const fs = require('fs');
const path = require('path');
const ImageTracer = require('imagetracerjs');
const { Jimp } = require('jimp');

const IMAGES_DIR = path.join(__dirname, 'images');
const OUTPUT_DIR = path.join(__dirname, 'output');

/**
 * Converts a raster image to SVG format using ImageTracer.
 */
async function convertToSvg(inputPath, outputPath) {
    console.log(`Converting: ${path.basename(inputPath)}...`);

    try {
        // Load image using Jimp
        const image = await Jimp.read(inputPath);

        // Preparation for ImageTracer: Get RGBA data
        const imageData = {
            width: image.bitmap.width,
            height: image.bitmap.height,
            data: image.bitmap.data
        };

        // Convert to SVG
        const svgString = ImageTracer.imagedataToSVG(imageData, 'detailed');

        // Save output
        fs.writeFileSync(outputPath, svgString);
        console.log(`Success: Saved to ${path.basename(outputPath)}`);
        return true;
    } catch (error) {
        console.error(`Error converting ${path.basename(inputPath)}:`, error.message);
        return false;
    }
}

async function main() {
    // Ensure output directory exists
    if (!fs.existsSync(OUTPUT_DIR)) {
        fs.mkdirSync(OUTPUT_DIR, { recursive: true });
    }

    // Ensure images directory exists
    if (!fs.existsSync(IMAGES_DIR)) {
        console.error(`Error: Directory not found: ${IMAGES_DIR}`);
        return;
    }

    // Read all files in the images directory
    const files = fs.readdirSync(IMAGES_DIR);
    const supportedExtensions = ['.jpg', '.jpeg', '.png', '.bmp'];
    const imageFiles = files.filter(file => supportedExtensions.includes(path.extname(file).toLowerCase()));

    if (imageFiles.length === 0) {
        console.warn('No supported image files found in the images directory.');
        return;
    }

    console.log(`Found ${imageFiles.length} images to process.\n`);

    for (const file of imageFiles) {
        const inputPath = path.join(IMAGES_DIR, file);
        const outputFileName = path.basename(file, path.extname(file)) + '.svg';
        const outputPath = path.join(OUTPUT_DIR, outputFileName);

        await convertToSvg(inputPath, outputPath);
    }

    console.log('\nAll tasks completed.');
}

main().catch(err => console.error('Script failed:', err));
