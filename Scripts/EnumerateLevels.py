import os
import sys

def rename_files(directory, numbers):
    numbers = [num.zfill(4) for num in numbers]

    # Step 1: First, rename the files using a temporary "TT" prefix to avoid conflicts.
    for index, old_num in enumerate(numbers):
        new_num = f"{index + 901:04}"  # Convert index to the desired format, e.g., 0101, 0102, etc.
        
        # Define the original and temporary file names
        old_png = os.path.join(directory, f"{old_num}.png")
        old_aux = os.path.join(directory, f"{old_num}.aux")
        temp_png = os.path.join(directory, f"TT{new_num}.png")
        temp_aux = os.path.join(directory, f"TT{new_num}.aux")

        print(f"Rename {old_num} -> {new_num}")
        
        # Rename files to the temporary name (if the files exist)
        if os.path.exists(old_png):
            os.rename(old_png, temp_png)
        if os.path.exists(old_aux):
            os.rename(old_aux, temp_aux)
    
    # Step 2: Rename the temporary files to the final target names.
    for index in range(len(numbers)):
        new_num = f"{index + 901:04}"  # Convert index to the desired format, e.g., 0101, 0102, etc.
        
        # Define the temporary and final file names
        temp_png = os.path.join(directory, f"TT{new_num}.png")
        temp_aux = os.path.join(directory, f"TT{new_num}.aux")
        final_png = os.path.join(directory, f"{new_num}.png")
        final_aux = os.path.join(directory, f"{new_num}.aux")
        
        # Rename temporary files to the final names (if the temp files exist)
        if os.path.exists(temp_png):
            os.rename(temp_png, final_png)
        if os.path.exists(temp_aux):
            os.rename(temp_aux, final_aux)

if __name__ == "__main__":
    # Get directory and list of numbers from command-line arguments
    directory = sys.argv[1]  # Directory containing the files
    numbers = sys.argv[2:]   # List of numbers for renaming
    
    rename_files(directory, numbers)
