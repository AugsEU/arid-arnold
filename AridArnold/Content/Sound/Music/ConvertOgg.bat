@echo off
if "%~1"=="" (
    echo Please drag and drop a .wav file onto this .bat file.
    pause
    exit /b
)

set "input=%~1"
set "output=%~dpn1.ogg"

ffmpeg -i "%input%" -c:a libvorbis -qscale:a 6 "%output%"