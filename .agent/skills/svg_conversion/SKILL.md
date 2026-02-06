---
name: SVG Image Vectorization
description: Convert raster images (PNG, JPG, BMP) into vector SVG format using Node.js.
---

# SVG Image Vectorization Skill

This skill allows the agent to vectorize raster images into SVG format using `imagetracerjs`. It provides a high-fidelity 'detailed' vectorization preset.

## Capabilities
- Convert PNG, JPG, and BMP to SVG.
- Batch process multiple images.
- Generate high-fidelity vector paths.

## Requirements
- **Node.js**: v18 or higher recommended.
- **Dependencies**: `imagetracerjs`, `jimp`.

## Usage

### 1. Identify the Image
Locate the absolute path to the image or directory of images you want to process.

### 2. Run the Vectorization Tool
The skill includes a pre-configured Node.js script. You can run it using:

```powershell
node .agent/skills/svg_conversion/scripts/index.js
```

*Note: The script defaults to looking in a local `images/` folder and outputting to `output/`. For specific files, the script can be modified or invoked via a wrapper.*

## Implementation Details
Uses `imagetracerjs` for the core vectorization logic and `jimp` for image data extraction in the Node.js environment.
