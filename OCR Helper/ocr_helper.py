import cv2
import pytesseract
from pytesseract import Output
import os
import numpy as np
from PIL import Image

def is_bold(img_gray, x, y, w, h, threshold_padding=2):
    """
    Check if a word is bold by comparing its mean intensity 
    to a slightly padded background area.
    """
    # Extract the word region
    roi = img_gray[y:y+h, x:x+w]
    if roi.size == 0:
        return False
    
    # Calculate mean intensity of the ROI (0=black, 255=white)
    word_mean = np.mean(roi)
    
    # We compare it to a slightly larger area to get local context if possible,
    # or just use a fixed heuristic if simple.
    # For now, let's use a simpler heuristic: lower mean = more ink = likely bold.
    # Standard text usually has a mean around 200-240 on white background.
    # Bold text will be significantly lower.
    
    # A more robust way is to threshold the image and count black pixels
    _, binary = cv2.threshold(roi, 0, 255, cv2.THRESH_BINARY_INV + cv2.THRESH_OTSU)
    ink_pixels = np.count_nonzero(binary)
    total_pixels = binary.size
    density = ink_pixels / total_pixels if total_pixels > 0 else 0
    
    return density

def process_image(image_path):
    print(f"Processing {image_path}...")
    
    # Load image
    img = cv2.imread(image_path)
    if img is None:
        print(f"Error: Could not read image {image_path}")
        return
        
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    
    # Get OCR data
    d = pytesseract.image_to_data(gray, output_type=Output.DICT)
    
    n_boxes = len(d['text'])
    lines = {}
    
    # First pass: collect densities to find a relative threshold
    densities = []
    for i in range(n_boxes):
        if int(d['conf'][i]) > 40: # Confidence threshold
            text = d['text'][i].strip()
            if text:
                density = is_bold(gray, d['left'][i], d['top'][i], d['width'][i], d['height'][i])
                densities.append(density)
                d['density'] = d.get('density', []) + [density]
            else:
                d['density'] = d.get('density', []) + [0]
        else:
            d['density'] = d.get('density', []) + [0]

    if not densities:
        print("No text found.")
        return

    # Heuristic: top 30% densest words might be bold if there's enough variation
    avg_density = np.mean(densities)
    std_density = np.std(densities)
    bold_threshold = avg_density + (0.5 * std_density) # Adjustable multiplier
    
    markdown_lines = []
    current_line_num = -1
    current_line_text = []

    for i in range(n_boxes):
        if int(d['conf'][i]) > 40:
            text = d['text'][i].strip()
            if not text:
                continue
                
            line_num = d['line_num'][i]
            word_density = d['density'][i]
            
            # Apply bold formatting
            if word_density > bold_threshold:
                processed_text = f"**{text}**"
            else:
                processed_text = text
            
            if line_num != current_line_num:
                if current_line_text:
                    markdown_lines.append(" ".join(current_line_text))
                current_line_text = [processed_text]
                current_line_num = line_num
            else:
                current_line_text.append(processed_text)
                
    if current_line_text:
        markdown_lines.append(" ".join(current_line_text))
        
    return "\n\n".join(markdown_lines)

def main():
    images_dir = "images"
    output_dir = "output"
    
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)
        
    image_files = [f for f in os.listdir(images_dir) if f.lower().endswith(('.png', '.jpg', '.jpeg', '.tiff', '.bmp'))]
    
    if not image_files:
        print(f"No images found in {images_dir}/")
        return

    for image_file in image_files:
        path = os.path.join(images_dir, image_file)
        md_content = process_image(path)
        
        if md_content:
            output_file = os.path.join(output_dir, os.path.splitext(image_file)[0] + ".md")
            with open(output_file, "w", encoding="utf-8") as f:
                f.write(md_content)
            print(f"Saved results to {output_file}")

if __name__ == "__main__":
    main()
