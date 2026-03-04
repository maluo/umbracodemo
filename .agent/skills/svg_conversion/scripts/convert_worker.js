const fs = require('fs');
const path = require('path');
const ImageTracer = require('imagetracerjs');
const { Jimp } = require('jimp');

/**
 * Converts a raster image to SVG format using ImageTracer with specific options.
 */
async function convertToSvg(inputPath, outputPath, options) {
    console.log(`Converting: ${path.basename(inputPath)} with high detail...`);

    try {
        const image = await Jimp.read(inputPath);
        const imageData = {
            width: image.bitmap.width,
            height: image.bitmap.height,
            data: image.bitmap.data
        };

        // options can include ltres, qtres, pathomit, numberofcolors, etc.
        const svgString = ImageTracer.imagedataToSVG(imageData, options);

        fs.writeFileSync(outputPath, svgString);
        console.log(`Success: Saved to ${path.basename(outputPath)} (${(svgString.length / 1024 / 1024).toFixed(2)} MB)`);
        return true;
    } catch (error) {
        console.error(`Error:`, error.message);
        return false;
    }
}

const inputPath = process.argv[2];
const outputPath = process.argv[3];

if (!inputPath || !outputPath) {
    console.error('Usage: node convert_worker.js <input> <output>');
    process.exit(1);
}

// We want maximum clarity, ignoring file size.
const highDetailOptions = {
    ltres: 0.01, // Extremely low error threshold for straight lines.
    qtres: 0.01, // Extremely low error threshold for quadratic splines.
    pathomit: 0, // Do not omit any edges.
    numberofcolors: 256, // Maximum colors for better gradients/outlines.
    mincolorratio: 0,
    colorquantcycles: 5, // More cycles for better color quantization.
    scale: 1,
    simplifythreshold: 0,
    roundcoords: 2, // 2 decimal places for better precision.
    lcpr: 0,
    qcpr: 0,
    desc: false,
    viewbox: true,
    blurradius: 0, // No smoothing.
    blurdelta: 0
};


convertToSvg(inputPath, outputPath, highDetailOptions);
