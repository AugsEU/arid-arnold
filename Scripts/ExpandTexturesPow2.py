import os
import argparse
from PIL import Image
import math

COMPRESSION_LIMIT = 100000

def next_power_of_2(x):
    """Returns the next power of 2 greater than or equal to x."""
    return 1 if x == 0 else 2**(x - 1).bit_length()

def resize_image_to_power_of_2(image_path):
    """Resizes the image to the nearest power of 2 and saves it over the original image."""
    with Image.open(image_path) as img:
        width, height = img.size

        if width * height < COMPRESSION_LIMIT:
            return
        

        print(f"        Expanding...")
        print(f"")

        new_width = next_power_of_2(width)
        new_height = next_power_of_2(height)
        
        # Create a new image with a transparent background
        new_img = Image.new("RGBA", (new_width, new_height), (0, 0, 0, 0))
        
        # Paste the original image onto the new image, top-left corner
        new_img.paste(img, (0, 0))
        
        # Save the new image
        new_img.save(image_path)

def process_directory(directory_path):
    """Recursively processes each .png file in the directory and subdirectories."""
    for root, _, files in os.walk(directory_path):
        for file in files:
            if file.lower().endswith('.png'):
                file_path = os.path.join(root, file)
                print(f"Processing {file_path}...")
                resize_image_to_power_of_2(file_path)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Resize .png images to the nearest power of 2.")
    parser.add_argument("directory", type=str, help="The directory path to process.")
    args = parser.parse_args()
    
    process_directory(args.directory)