import os
import sys
import chardet

def detect_encoding(file_path):
    """Detect file encoding."""
    with open(file_path, 'rb') as f:
        raw_data = f.read()
        result = chardet.detect(raw_data)
        return result['encoding']

def compile_txt_files(input_dir):
    compiled_content = []
    for root, dirs, files in os.walk(input_dir):
        for file in files:
            if file.endswith('.txt'):
                file_path = os.path.join(root, file)
                #compiled_content.append(f"\n===============\n{file_path}\n===============\n")
                compiled_content.append(f"\n\n")

                # Detect encoding of the file
                encoding = detect_encoding(file_path)
                if encoding is None:
                    print(f"Warning: Unable to detect encoding for {file_path}. Skipping file.")
                    continue

                # Open the file with the detected encoding
                try:
                    with open(file_path, 'r', encoding=encoding) as f:
                        compiled_content.append(f.read())
                except Exception as e:
                    print(f"Error reading {file_path}: {e}")
                    continue

    # Join all the contents into one large string
    full_text = "\n".join(compiled_content)

    # Save the compiled content into Compiled.txt in the same directory
    output_file = os.path.join(input_dir, 'Compiled.txt')
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(full_text)

    print(f"Compiled content saved to: {output_file}")

# Main script
if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python script.py <directory_path>")
        sys.exit(1)

    directory = sys.argv[1]
    if not os.path.isdir(directory):
        print(f"Error: {directory} is not a valid directory.")
        sys.exit(1)

    compile_txt_files(directory)