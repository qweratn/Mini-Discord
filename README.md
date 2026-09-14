# Mini Discord

Mini Discord is a full-stack real-time chat application inspired by Discord. It supports direct chats, server chats, user search, member management, message history, and live message updates with SignalR.

![Mini Discord home page](Frontend/public/hero.png)

## Stack

### Backend

- .NET 10 / ASP.NET Core Web API
- Clean Architecture: Domain, Application, Infrastructure, Presentation
- Entity Framework Core and PostgreSQL
- MediatR and FluentValidation
- SignalR
- Clerk JWT authentication
- xUnit, Testcontainers

### Frontend

- React 19 and TypeScript
- Vite
- React Router
- Clerk React
- Axios
- `@microsoft/signalr`
- Tailwind CSS, shadcn/Base UI, and Lucide icons

## Architecture

- `Backend.Domain` — entities, business rules, and domain events.
- `Backend.Application` — commands, queries, handlers, validators, DTOs, and interfaces.
- `Backend.Infrastructure` — EF Core, PostgreSQL, repositories, migrations, and the transactional outbox.
- `Backend.Presentation` — controllers, authentication, CORS, Swagger, SignalR hub, and background services.
- `Frontend/src` — pages, shared components, API clients, authentication, chat UI, and SignalR integration.

When a chat member joins or a message is sent, the backend saves the domain data and an outbox record in the same transaction. A background service then publishes the event to the appropriate SignalR group.

## Requirements

- .NET 10 SDK
- Node.js 20.9+
- PostgreSQL 14+
- Docker
- A Clerk application

## Local setup

### 1. Database

Create a PostgreSQL database named `MiniDiscord` and update the connection string in `Backend/Backend.Presentation/appsettings.Development.json` or through environment variables:

```bash
export ConnectionStrings__PostgreSql="Host=localhost;Port=5432;Database=MiniDiscord;Username=postgres;Password=YOUR_PASSWORD"
export Clerk__Authority="https://YOUR_CLERK_INSTANCE.clerk.accounts.dev"
```

### 2. Backend

```bash
cd Backend
dotnet tool restore
dotnet restore Backend.sln
dotnet run --project Backend.Presentation --launch-profile https
```

The backend is available at `https://localhost:7260`.

- Swagger: `https://localhost:7260/swagger`
- REST API: `https://localhost:7260/api`
- SignalR hub: `https://localhost:7260/hubs/chat`

EF Core migrations are applied automatically on startup.

### 3. Frontend

Create `Frontend/.env`:

```env
VITE_CLERK_PUBLISHABLE_KEY=pk_test_YOUR_KEY
VITE_API_URL=https://localhost:7260/api
VITE_SIGNALR_URL=https://localhost:7260/hubs/chat
```

Then run:

```bash
cd Frontend
npm install
npm run dev
```

Open `http://localhost:5173` in your browser.

If HTTPS is not trusted locally, run:

```bash
dotnet dev-certs https --trust
```

## Main API routes

| Method | Route | Purpose |
|---|---|---|
| `PUT` | `/api/users/sync` | Synchronize the Clerk user with the local database |
| `GET` | `/api/users/search?filter=` | Search users |
| `GET` | `/api/chats` | Get the current user's chats |
| `POST` | `/api/chats/direct` | Create or get a direct chat |
| `POST` | `/api/chats/server` | Create a server chat |
| `PUT` | `/api/chats/{chatId}/members/{userId}` | Add a member to a server |
| `GET` | `/api/chats/{chatId}/members` | Get chat members |
| `GET` | `/api/chats/{chatId}/messages` | Get message history |
| `POST` | `/api/chats/{chatId}/messages` | Send a message |

Protected routes require a Clerk JWT in the `Authorization: Bearer <token>` header.

## SignalR

The frontend connects to `/hubs/chat` and calls `SubscribeToChat(chatId)` after verifying chat membership. The server publishes:

- `MessageReceived` — a new message was sent;
- `ChatMemberJoined` — a user joined the chat.

## Tests

Run all backend tests from `Backend`:

```bash
dotnet test Backend.sln
```

Integration tests start a temporary PostgreSQL container, so Docker must be running.

Frontend checks:

```bash
cd Frontend
npm run lint
npm run build
```
