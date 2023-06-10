#!/usr/bin/env python

import sys
import shutil
import os

def clone_files(code, num_duplicates):
    starting_number = int(code) + 1
    for i in range(num_duplicates):
        new_code = str(starting_number + i).zfill(4)
        original_files = [f"{code}.png", f"{code}.aux"]
        new_files = [f"{new_code}.png", f"{new_code}.aux"]
        
        for original_file, new_file in zip(original_files, new_files):
            shutil.copy(original_file, new_file)
            print(f"Cloned {original_file} as {new_file}")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python CloneLevel.py <code> <num_duplicates>")
    else:
        dir_path = os.path.dirname(os.path.realpath(__file__))
        os.chdir(dir_path + '/../AridArnold/Content/Campaigns/MainCampaign/Hub')

        code = sys.argv[1]
        num_duplicates = int(sys.argv[2])
        clone_files(code, num_duplicates)