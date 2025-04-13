#!/bin/bash

PID_FILE="./server.pid"

cd "./voice" || { echo "Directory /voice not found"; exit 1; }

# Check if the reference audio file exists
if [ ! -f ./voice.wav ]; then
    echo "Please provide a reference audio file named voice.wav"
    exit 1
fi

# Function to determine the input text
get_input_text() {
    if [ "$1" ]; then
        input_text=$1
    elif [ -f input.txt ]; then
        input_text=$(cat input.txt)
    else
        echo "Please provide a text to convert to speech"
        exit 1
    fi
}

# Function to set up the virtual environment
check_environment() {
    if [ ! -d ./voiceenv ]; then
        echo "Creating virtual environment"
        python3 -m venv voiceenv
        source ./voiceenv/bin/activate
        pip install --upgrade pip
        pip install f5-tts-mlx fastapi uvicorn
        deactivate
    else
        echo "Virtual environment already exists"
    fi
}

# Function to start the server
start_server() {
    if [ -f "$PID_FILE" ] && kill -0 $(cat "$PID_FILE") 2>/dev/null; then
        echo "Server is already running (PID: $(cat $PID_FILE))"
        exit 0
    fi

    echo "Starting server..."
    check_environment
    source ./voiceenv/bin/activate
    nohup python3 -m server > server.log 2>&1 &
    echo $! > "$PID_FILE"
    echo "Server started (PID: $(cat $PID_FILE))"
    deactivate
}

# Function to stop the server
stop_server() {
    if [ -f "$PID_FILE" ] && kill -0 $(cat "$PID_FILE") 2>/dev/null; then
        echo "Stopping server (PID: $(cat $PID_FILE))..."
        kill $(cat "$PID_FILE") && rm -f "$PID_FILE"
        echo "Server stopped"
    else
        echo "Server is not running"
    fi
}

# Function to check server status
status_server() {
    if [ -f "$PID_FILE" ] && kill -0 $(cat "$PID_FILE") 2>/dev/null; then
        echo "Server is running (PID: $(cat $PID_FILE))"
    else
        echo "Server is not running"
    fi
}

restart_server() {
    stop_server
    start_server
}

# Main script logic
case "$1" in
    start)
        start_server
        ;;
    stop)
        stop_server
        ;;
    status)
        status_server
        ;;
    restart)
        restart_server
        ;;
    *)
        echo "Usage: $0 {start|stop|status}"
        exit 1
        ;;
esac