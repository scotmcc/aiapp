# AI Project

## Purpose

This repository serves as a baseline for an AI project, providing a comprehensive setup with various technologies. It is designed to be a starting point for developers looking to build AI applications with a robust architecture. The framework assumes a basic understanding of Docker, C#, and the technologies used in this project. As well as a basic understanding of AI and machine learning concepts. The project is not intended to be a complete solution but rather a foundation for further development and experimentation. It is also assumed that the user will be using Ollama's LLMs, which are open source and available for free. The project is designed to be modular and extensible, allowing developers to easily add new features and functionality as needed. The goal is to provide a solid starting point for building AI applications that can be deployed in a production environment.

## Description

This repository contains an AI project built with the following technologies:

-   **Docker**: Containerization platform for managing services.
-   **C#**: Backend API and Blazor frontend.
-   **Nginx**: Reverse proxy server.
-   **Postgres**: Relational database.
-   **PgAdmin**: Database management tool for Postgres.
-   **Qdrant**: Vector search engine.
-   **Redis**: In-memory data store.
-   **RedisInsight**: GUI for managing Redis.
-   **Tailscale**: VPN for secure networking.

## Features

-   **AI-Powered Functionality**: AI capabilities integrated into the application.
-   **Scalable Architecture**: Built with Docker for easy deployment.
-   **Secure Networking**: Tailscale integration for secure connections.

## Prerequisites

-   Docker and Docker Compose installed.
-   [.NET SDK](https://dotnet.microsoft.com/download) installed for local development.
-   Access to Tailscale for secure networking.

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/yourusername/your-repo-name.git
cd your-repo-name
```

### Environment Variables

Create a `.env` file in the root directory with the following variables:

```env
API_BASE_URL=your_api_base_url
HOSTNAME=your_hostname
POSTGRES_USER=your_user
POSTGRES_PASSWORD=your_password
POSTGRES_DB=your_database
TS_AUTHKEY=your_tailscale_auth_key
PGADMIN_EMAIL=your_pgadmin_email
PGADMIN_PASSWORD=your_pgadmin_password
```

### Build and Run

Use Docker Compose to build and run the services:

```bash
./scripts/aiapp.sh -h # for help
./scripts/aiapp.sh start # to start the services
./scripts/aiapp.sh stop # to stop the services
```

### Access the Application

-   **Blazor Frontend**: `http://${your_api_base_url}`
-   **API**: `http://${your_api_base_url}/api`
-   **PgAdmin**: `http://${your_api_base_url}/admin`
-   **RedisInsight**: `http://${your_api_base_url}/redis`
-   **Qdrant**: `http://${your_api_base_url}/qdrant/dashboard`

## Project Structure

```
.
├── docs # Documentation files
├── LICENSE # License file
├── docker # Docker configuration files
│   ├── nginx
│   │   ├── conf.d
│   │   ├── html
│   │   └── logs
│   ├── pgadmin
│   ├── postgres
|   │   ├── data
│   │   └── init
│   ├── qdrant
│   │   ├── config
│   │   └── storage
│   ├── redis
│   │   └── data
│   ├── redisinsight
│   ├── tailscale
│   │   └── config
│   └── webui
│       └── keys
├── scripts # Scripts for automation
└── src # Source code for the project
    ├── api # C# API project
    │   ├── Controllers
    │   ├── Data
    │   ├── Migrations
    │   ├── Properties
    └── gui # C# Blazor project
        ├── Components
        ├── Properties
        └── wwwroot
            └── dist # UI files
                ├── bootstrap
                │   ├── css
                │   └── js
                └── fontawesome
                    ├── css
                    └── webfonts

```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the [UNLICENSE License](LICENSE).
