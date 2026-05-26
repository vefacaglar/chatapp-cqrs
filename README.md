# ChatApp CQRS

A chat application built on .NET 10 using CQRS (Command Query Responsibility Segregation) and Event Sourcing architecture.

## Tech Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Runtime |
| PostgreSQL | 16 | Write Database |
| MongoDB | 7 | Read Database |
| RabbitMQ | 3 | Event Bus |
| Entity Framework Core | 10.0 | ORM |
| CustomDispatcher | 1.0.0 | CQRS Dispatch |
| Scalar | 2.4.1 | API Documentation |

## Projects

| Project | Description |
|---------|-------------|
| `ChatApp.Api` | REST API endpoints, DI configuration |
| `ChatApp.Application` | Command/Query handlers, Event handlers, Middleware |
| `ChatApp.Domain` | Entities, Value Objects, Enums |
| `ChatApp.Infrastructure` | EF Core, Repository, Event Store, RabbitMQ |
| `ChatApp.Test` | Unit tests |

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
