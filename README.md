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

# JS applications only
pnpm dev

# .NET API
dotnet run --project apps/api/ChatApp.Api
```

**Terminal 2 - Socket.IO Bridge + React Client (via Turborepo):**
```bash
npm run dev
```

Or run them individually:

```bash
# Socket.IO Bridge only
npx turbo dev --filter=@chatapp/socket-bridge

# React Client only
npx turbo dev --filter=@chatapp/client
```

### Build for Production

```bash
# All JS applications
pnpm build

# Single application
pnpm --filter @chatapp/client build
```

### Service URLs

| Service | URL |
|---------|-----|
| React Client | http://localhost:3000 |
| Socket.IO Bridge | http://localhost:3001 |
| .NET API | http://localhost:5268 |
| API Documentation | http://localhost:5268/scalar/v1 |
| RabbitMQ Management | http://localhost:15672 |

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
  redis:       # localhost:6379
```

## License

MIT
