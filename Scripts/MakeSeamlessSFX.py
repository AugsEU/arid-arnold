import math
import sys
import os
from pydub import AudioSegment
from pydub.playback import play

SAMPLE_SIZE_MULT = 1024

def round_down_to_sample(number):
    return int(math.floor(number / 1024) * 1024)

def create_seamless_loop(input_file, output_file):
    # Load the audio file
    sound = AudioSegment.from_wav(input_file)
    
    duration = len(sound)

    ideal_duration = round_down_to_sample(duration - (duration // 10))
    fade_length = duration - ideal_duration

    # Calc end chunk
    end_chunk = sound[-fade_length:]
    end_chunk = end_chunk.fade_out(int(fade_length * 0.65))
    
    # Cut the original sound by removing the last 10%
    start_chunk = sound[:-fade_length]
    start_chunk = start_chunk.fade_in(int(fade_length * 0.65))
    
    # Combine the mixed part with the remaining original sound
    final_sound = start_chunk.overlay(end_chunk)
    
    # Export the new sound
    final_sound.export(output_file, format="wav")
    
    print(f"Seamless loop created and saved as {output_file}")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python script.py <input_wav_path>")
        sys.exit(1)
    
    input_file = sys.argv[1]
    
    # Generate the output file name by appending "_seamless" before the file extension
    base_name, ext = os.path.splitext(input_file)
    output_file = f"{base_name}_seamless{ext}"

    create_seamless_loop(input_file, output_file)