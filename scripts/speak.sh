#!/bin/bash

# This script uses the f5-tts-mlx library to generate speech from text
# It requires a reference audio file named voice.wav to be present in the same directory
# The reference audio file should be a recording of the speaker saying the reference text
# The reference text should be a sentence that the speaker is likely to say in the generated speech
# The script takes a text input and generates speech from it using the reference audio file

if [ -z "$1" ]; then
    echo "Please provide a text to convert to speech"
    exit 1
fi

cd ".src/Voice"

if [ ! -f ./voice.wav ]; then
    echo "Please provide a reference audio file named voice.wav"
    exit 1
fi

if [ ! -f ./voiceenv/bin/activate ]; then
    echo "Creating virtual environment"
    python3 -m venv voiceenv
    source ./voiceenv/bin/activate
    pip install f5-tts-mlx
    deactivate
fi

source ./voiceenv/bin/activate
python3 -m f5_tts_mlx.generate \
--text "$1" \
--ref-audio ./voice.wav \
--ref-text "Hey guys, welcome back to popcorn in bed. Thank you as always so much for being here." \
--output ../src/wwwroot/audiooutput.wav
deactivate
