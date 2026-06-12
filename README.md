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
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0 | PostgreSQL provider |
| MongoDB.Driver | 3.3.0 | MongoDB client |
| RabbitMQ.Client | 7.1.2 | RabbitMQ client |
| StackExchange.Redis | 2.8.16 | Redis client |
| Polly | 8.5.2 | Resilience/retry policies |
| Newtonsoft.Json | 13.0.3 | JSON serialization |
| CustomDispatcher | 1.0.0 | CQRS Dispatch |
| Scalar | 2.4.1 | API Documentation |

### Frontend (JavaScript)

| Technology | Version | Purpose |
|------------|---------|---------|
| React | 19 | UI framework |
| Vite | 8 | Build tool |
| Tailwind CSS | 4 | Styling |
| Socket.IO Client | 4.8 | Real-time communication |
| Axios | 1 | HTTP client |
| React Router | 7 | Client-side routing |
| react-hot-toast | 2.6 | Toast notifications |
| Turborepo | 2 | Monorepo build system |

### Testing

| Technology | Version | Purpose |
|------------|---------|---------|
| xUnit | 2.9.3 | Test framework |
| Moq | 4.20.72 | Mocking library |
| Coverlet | 6.0.4 | Code coverage |

### Socket Bridge (Node.js)

| Technology | Version | Purpose |
|------------|---------|---------|
| Socket.IO | 4.7 | WebSocket server |
| ioredis | 5.4 | Redis pub/sub subscriber |

## Projects

| Project | Path | Description |
|---------|------|-------------|
| `ChatApp.Api` | `apps/api/ChatApp.Api` | REST API entry point |
| `ChatApp.Application` | `apps/api/ChatApp.Application` | Command/Query handlers, Event handlers, Middleware |
| `ChatApp.Domain` | `apps/api/ChatApp.Domain` | Entities, Value Objects, Enums |
| `ChatApp.Infrastructure` | `apps/api/ChatApp.Infrastructure` | EF Core, Repository, Event Store, RabbitMQ, Redis |
| `ChatApp.Test` | `apps/api/ChatApp.Test` | Unit tests |
| `@chatapp/client` | `apps/client` | React + Vite frontend (Turborepo workspace) |
| `@chatapp/socket-bridge` | `apps/socket-bridge` | Redis-to-Socket.IO bridge (Turborepo workspace) |

## Project Structure

```
chatapp-cqrs/
├── apps/
│   ├── api/
│   │   ├── ChatApp.Api/              ← REST API entry point
│   │   ├── ChatApp.Application/      ← CQRS handlers, event handlers
│   │   ├── ChatApp.Domain/           ← Entities, value objects
│   │   ├── ChatApp.Infrastructure/   ← EF Core, RabbitMQ, Redis
│   │   └── ChatApp.Test/             ← Unit tests
│   ├── client/                       ← React + Vite + Tailwind
│   └── socket-bridge/                ← Node.js Socket.IO bridge
├── docker-compose.yml                ← PostgreSQL, MongoDB, RabbitMQ, Redis
├── turbo.json                        ← Turborepo config
└── package.json                      ← JS workspaces
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/)
- [Node.js 20+](https://nodejs.org/) (pnpm is installed automatically)

### One-Command Setup and Run

Install and start everything with a single command:

```bash
bash scripts/setup.sh
```

This script:
1. Installs pnpm automatically if not already installed
2. Installs JS dependencies (pnpm install)
3. Restores NuGet packages
4. Starts Docker services (PostgreSQL, MongoDB, RabbitMQ, Redis)
5. Applies database migrations

Then start all services:

```bash
pnpm dev:all
```

This command starts the .NET API, Socket.IO Bridge, and React Client in parallel.

### Running Individually

```bash
# Docker infrastructure
pnpm dev:infra

# JS applications (Socket.IO Bridge + React Client)
pnpm dev

# Or run them individually
pnpm --filter @chatapp/socket-bridge dev
pnpm --filter @chatapp/client dev

# .NET API
dotnet run --project apps/api/ChatApp.Api
```

### Build for Production

```bash
# All JS applications
pnpm build

# Single application
pnpm --filter @chatapp/client build
```

### Service URLs

| Service | URL | Notes |
|---------|-----|-------|
| React Client | http://localhost:3000 | Proxies `/api` to .NET API |
| Socket.IO Bridge | http://localhost:3001 | WebSocket server |
| .NET API | http://localhost:5268 | REST API |
| API Documentation | http://localhost:5268/scalar/v1 | Scalar/OpenAPI docs |
| RabbitMQ Management | http://localhost:15672 | guest/guest |

## Environment Variables

### .NET API

Configuration is read from `appsettings.json` and `appsettings.{Environment}.json`. Connection strings and service endpoints can be overridden via environment variables using standard .NET configuration binding:

| Variable | Default | Description |
|----------|---------|-------------|
| `ConnectionStrings__ChatDbCommand` | `Host=localhost;...` | PostgreSQL write database |
| `ConnectionStrings__EventBus__Connection` | `localhost` | RabbitMQ host |
| `ConnectionStrings__EventBus__UserName` | `guest` | RabbitMQ username |
| `ConnectionStrings__EventBus__Password` | `guest` | RabbitMQ password |
| `MongoDb__ConnectionString` | `mongodb://localhost:27017` | MongoDB connection string |
| `MongoDb__DatabaseName` | `ChatDbRead` | MongoDB database name |
| `Redis__ConnectionString` | `localhost:6379` | Redis connection string |
| `RetryCount` | `5` | RabbitMQ retry attempts |

### Socket Bridge

| Variable | Default | Description |
|----------|---------|-------------|
| `REDIS_HOST` | `localhost` | Redis host for pub/sub subscription |
| `REDIS_PORT` | `6379` | Redis port |
| `SOCKETIO_PORT` | `3001` | Socket.IO server port |
| `CORS_ORIGIN` | `*` | CORS allowed origin |

### React Client

| Variable | Default | Description |
|----------|---------|-------------|
| `VITE_SOCKET_URL` | `http://localhost:3001` | Socket.IO bridge URL |

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
1. POST /api/v1/chat  ──▶  LoggingDispatchMiddleware
                              │
                              ├─▶ Log request + persist to EventLog (PostgreSQL)
                              └─▶ Dispatch CreateChatRoomCommand
                                    │
                                    ▼
                              Write to PostgreSQL
                                    │
                                    ├─▶ Publish EventCreatedChatRoom (RabbitMQ)
                                    │     │
                                    │     ▼
                                    │   CreatedChatRoomEventHandler
                                    │     │
                                    │     ├─▶ Write to MongoDB (Read Model)
                                    │     └─▶ Publish to Redis (chat:room:created)
                                    │           │
                                    │           ▼
                                    │         Socket.IO Bridge → WebSocket → Client
                                    │
                                    └─▶ Return { code: "guid" }

2. GET /api/v1/chat/{id}  ──▶  LoggingDispatchMiddleware
                                    │
                                    ├─▶ Log request + persist to EventLog (PostgreSQL)
                                    └─▶ Dispatch GetChatRoomByIdQuery
                                          │
                                          ▼
                                    Read from MongoDB
```

### Event Sourcing

Every command and query is logged to the `EventLog` table in PostgreSQL via `LoggingDispatchMiddleware`, providing a complete audit trail. Domain events (`EventCreatedChatRoom`, `MessageSentEvent`) are published to RabbitMQ and consumed by event handlers that update the MongoDB read model. The event bus uses Polly for retry with exponential backoff.

## Database Migrations

```bash
# Apply EF Core migrations manually
dotnet ef database update \
  --project apps/api/ChatApp.Infrastructure \
  --startup-project apps/api/ChatApp.Api
```

Or let `bash scripts/setup.sh` handle it automatically.

## Tests

```bash
dotnet test
```

### Code Coverage

```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# Install report generator (once)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator \
  -reports:"apps/api/ChatApp.Test/TestResults/**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html

# Open in browser
open coveragereport/index.html
```

> **Note:** If `reportgenerator` command is not found, add `~/.dotnet/tools` to your PATH:
> ```bash
> echo 'export PATH="$HOME/.dotnet/tools:$PATH"' >> ~/.zshrc && source ~/.zshrc
> ```

## Production

For production deployment, create override files for each service:

- **Backend:** `apps/api/ChatApp.Api/appsettings.Production.json` — override connection strings and `RetryCount`
- **Client:** Set `VITE_SOCKET_URL` to the production Socket.IO bridge URL
- **Socket Bridge:** Set `REDIS_HOST`, `REDIS_PORT`, `SOCKETIO_PORT`, and `CORS_ORIGIN` for the production environment
- **Infrastructure:** Use `docker-compose.override.yml` or environment-specific compose files

Build the JS applications for production:

```bash
pnpm build
```

Publish the .NET API:

```bash
dotnet publish apps/api/ChatApp.Api -c Release -o out
```

## Docker Compose Services

```yaml
services:
  rabbitmq:    # localhost:5672, Management: localhost:15672
  postgres:    # localhost:5432
  mongodb:     # localhost:27017
  redis:       # localhost:6379
```

## License

MIT
