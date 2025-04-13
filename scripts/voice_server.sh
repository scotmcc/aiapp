#!/bin/bash
# filepath: /Users/scot/Projects/F5-TTS_server/manage_server.sh

SERVER_COMMAND="uvicorn f5-tts_server.server:app --host 0.0.0.0 --port 7860"
PID_FILE="server.pid"

cd "$PWD/../F5-TTS_server" || exit 1

# Check if the server is already running
if [ -f "$PID_FILE" ]; then
    if ps -p $(cat $PID_FILE) > /dev/null; then
        echo "Server is already running (PID: $(cat $PID_FILE))."
        exit 0
    else
        echo "Stale PID file found. Removing..."
        rm -f $PID_FILE
    fi
fi

# Create Python virtual environment if it doesn't exist
if [ ! -d "venv" ]; then
    echo "Creating Python virtual environment..."
    python3 -m venv venv
fi

# Activate the virtual environment
source venv/bin/activate
if [ $? -ne 0 ]; then
    echo "Failed to activate virtual environment."
    exit 1
fi

# Install required packages
if [ ! -f "requirements.txt" ]; then
    echo "requirements.txt not found. Please ensure it exists."
    exit 1
fi
pip install --upgrade pip
pip install -r requirements.txt
if [ $? -ne 0 ]; then
    echo "Failed to install required packages."
    deactivate
    exit 1
fi
# Deactivate the virtual environment
deactivate

# Check if the server command is valid
if ! command -v uvicorn &> /dev/null; then
    echo "uvicorn could not be found. Please install it."
    exit 1
fi

# Check if the server command is valid
if ! command -v python3 &> /dev/null; then
    echo "python3 could not be found. Please install it."
    exit 1
fi

# Function to start the server
start_server() {
    if [ -f "$PID_FILE" ]; then
        echo "Server is already running (PID: $(cat $PID_FILE))."
    else
        echo "Starting the F5-TTS server..."
        nohup $SERVER_COMMAND > server.log 2>&1 &
        echo $! > $PID_FILE
        echo "Server started (PID: $(cat $PID_FILE)). Logs are in server.log."
    fi
}

# Function to stop the server
stop_server() {
    if [ -f "$PID_FILE" ]; then
        echo "Stopping the server (PID: $(cat $PID_FILE))..."
        kill -9 $(cat $PID_FILE) && rm -f $PID_FILE
        echo "Server stopped."
    else
        echo "Server is not running."
    fi
}

# Function to display the menu
show_menu() {
    echo ""
    echo "Options:"
    echo "1. Start Server"
    echo "2. Stop Server"
    echo "3. Exit"
    echo -n "Enter your choice: "
}

# Main loop
while true; do
    show_menu
    read choice
    case $choice in
        1) start_server ;;
        2) stop_server ;;
        3) 
            if [ -f "$PID_FILE" ]; then
                stop_server
            fi
            echo "Exiting..."
            exit 0
            ;;
        *) echo "Invalid choice. Please try again." ;;
    esac
done