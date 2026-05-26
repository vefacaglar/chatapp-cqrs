# ChatApp CQRS

A chat application built on .NET 10 using CQRS (Command Query Responsibility Segregation) and Event Sourcing architecture.

## Tech Stack

### Backend (.NET)

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Runtime |
| PostgreSQL | 16 | Write Database |
| MongoDB | 7 | Read Database |
| RabbitMQ | 3 | Event Bus |
| Redis | 7 | Pub/Sub (Socket.IO bridge) |
| Entity Framework Core | 10.0 | ORM |
| StackExchange.Redis | 2.8.16 | Redis client |
| CustomDispatcher | 1.0.0 | CQRS Dispatch |
| Scalar | 2.4.1 | API Documentation |

### Frontend (JavaScript)

| Technology | Version | Purpose |
|------------|---------|---------|
| React | 19 | UI framework |
| Vite | 8 | Build tool |
| Tailwind CSS | 4 | Styling |
| Socket.IO Client | 4 | Real-time communication |
| Axios | 1 | HTTP client |
| React Router | 7 | Client-side routing |
| Turborepo | 2 | Monorepo build system |

### Socket Bridge (Node.js)

| Technology | Version | Purpose |
|------------|---------|---------|
| Socket.IO | 4 | WebSocket server |
| ioredis | 5 | Redis pub/sub subscriber |

## Projects

| Project | Description |
|---------|-------------|
| `ChatApp.Api` | REST API endpoints, DI configuration |
| `ChatApp.Application` | Command/Query handlers, Event handlers, Middleware |
| `ChatApp.Domain` | Entities, Value Objects, Enums |
| `ChatApp.Infrastructure` | EF Core, Repository, Event Store, RabbitMQ, Redis |
| `ChatApp.Test` | Unit tests |
| `apps/client` | React + Vite frontend (Turborepo workspace) |
| `apps/socket-bridge` | Redis-to-Socket.IO bridge (Turborepo workspace) |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/)

### Start Services

```bash
docker compose up -d
```

This command starts:
- **PostgreSQL** - `localhost:5432` (user: `postgres`, pass: `postgres`)
- **MongoDB** - `localhost:27017`
- **RabbitMQ** - `localhost:5672` (AMQP), `localhost:15672` (Management UI)

### Run Migrations

```bash
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate \
  --project src/ChatApp.Infrastructure \
  --startup-project src/ChatApp.Api

dotnet ef database update \
  --project src/ChatApp.Infrastructure \
  --startup-project src/ChatApp.Api
```

### Run the Application

```bash
dotnet run --project src/ChatApp.Api/ChatApp.Api.csproj
```

### API Documentation

```
http://localhost:5000/scalar/v1
```

## API Endpoints

### Command (Write)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/v1/chat` | Create a new chat room |
| `POST` | `/api/v1/chat/message` | Send a message |

### Query (Read - MongoDB)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/v1/chat` | List all chat rooms |
| `GET` | `/api/v1/chat/{id}` | Get a chat room by ID |

## CQRS Flow

```
1. POST /api/v1/chat  ──▶  CreateChatRoomCommand
                              │
                              ▼
                        Write to PostgreSQL
                              │
                              ▼
                        Publish EventCreatedChatRoom (RabbitMQ)
                              │
                              ▼
                        CreatedChatRoomEventHandler
                              │
                              ▼
                        Write to MongoDB (Read Model)

2. GET /api/v1/chat/{id}  ──▶  GetChatRoomByIdQuery
                                    │
                                    ▼
                              Read from MongoDB
```

## Tests

```bash
dotnet test
```

## Docker Compose Services

```yaml
services:
  rabbitmq:    # localhost:5672, Management: localhost:15672
  postgres:    # localhost:5432
  mongodb:     # localhost:27017
```

## License

MIT
