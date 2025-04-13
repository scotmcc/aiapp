#!/bin/bash

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "Docker is not running. Please start Docker and try again."
    exit 1
fi

# Check for compose command
compose_command() {
    if command -v docker-compose > /dev/null 2>&1; then
        echo "docker-compose"
    elif command -v docker compose > /dev/null 2>&1; then
        echo "docker compose"
    else
        echo "Docker Compose is not installed. Please install Docker Compose and try again."
        exit 1
    fi
}

# Clear the logs
clear_logs() {
    echo "Clearing logs..."
    if [ -d "./docker/nginx/logs" ]; then
        rm -rf ./docker/nginx/logs/*.*
        rm -rf ./docker/nginx/logs/.*
    else
        mkdir -p ./docker/nginx/logs
    fi
    if [ -d "./docker/redisinsight/logs" ]; then
    rm -rf ./docker/redisinsight/logs/*.*
        rm -rf ./docker/redisinsight/logs/.*
    else
        mkdir -p ./docker/redisinsight/logs
    fi
}

# Start Docker containers
start_services() {
    echo "Starting Docker containers..."
    # Clear logs if the flag is set
    if [ "$clear_logs_flag" = true ]; then
        clear_logs
    fi
    $(compose_command) up -d --remove-orphans
}

# Stop Docker containers
stop_services() {
    echo "Stopping Docker containers..."
    $(compose_command) down
    # Clear logs if the flag is set
    if [ "$clear_logs_flag" = true ]; then
        clear_logs
    fi
    dotnet clean
}

watch_services() {
    echo "Watching Docker containers..."
    # Clear logs if the flag is set
    if [ "$clear_logs_flag" = true ]; then
        clear_logs
    fi
    dotnet restore
    dotnet restore --source ./src/AIApp.csproj 
    $(compose_command) up --watch
}

# Check the status of Docker containers
check_status() {
    echo "Checking Docker container status..."
    $(compose_command) ps
}

# Display help information
show_help() {
    echo "Usage: $0 [-c] [start|stop|status|help]"
    echo
    echo "Options:"
    echo "  -c      Clear logs after executing the command"
    echo
    echo "Commands:"
    echo "  start   Start Docker containers"
    echo "  stop    Stop Docker containers"
    echo "  watch   Watch Docker containers"
    echo "  status  Check the status of Docker containers"
    echo "  help    Show this help message"
}

# Parse options and arguments
clear_logs_flag=false
while getopts ":c" opt; do
    case $opt in
        c)
            clear_logs_flag=true
            ;;
        *)
            show_help
            exit 1
            ;;
    esac
done
shift $((OPTIND - 1))

# Main script logic
if [ $# -eq 0 ]; then
    show_help
    exit 1
fi

case $1 in
    start)
        start_services
        ;;
    stop)
        stop_services
        ;;
    watch)
        watch_services
        ;;
    status)
        check_status
        ;;
    help)
        show_help
        exit 0
        ;;
    *)
        echo "Invalid command: $1"
        show_help
        exit 1
        ;;
esac
