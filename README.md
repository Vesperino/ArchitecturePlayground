# ArchitecturePlayground

> E-Commerce Platform demonstrating enterprise architecture patterns with .NET 9 + Vue 3

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![Vue](https://img.shields.io/badge/Vue-3.x-4FC08D?logo=vue.js)
![License](https://img.shields.io/badge/license-MIT-blue)

## What This Project Demonstrates

| Category | Technologies & Patterns |
|----------|-------------------------|
| **Architecture** | Modular Monolith, Vertical Slice, DDD, CQRS |
| **Backend** | .NET 9, ASP.NET Core Minimal APIs, MediatR, FluentValidation |
| **Frontend** | Vue 3 (Composition API), TypeScript, Pinia, TailwindCSS |
| **Databases** | PostgreSQL, MongoDB, Redis |
| **Messaging** | MassTransit, RabbitMQ, Outbox Pattern |
| **Security** | OAuth 2.0, JWT, OWASP Top 10 |
| **DevOps** | Docker, GitHub Actions |
| **Cloud** | Azure, Supabase, MongoDB Atlas, Upstash, CloudAMQP |
| **Testing** | xUnit, Testcontainers, Playwright, NetArchTest |

## Architecture

**Modular Monolith + Vertical Slice Architecture** - the pragmatic 2025 approach:

```
┌─────────────────────────────────────────────────────────────────────┐
│                         HOST (API)                                   │
│                    ArchitecturePlayground.API                        │
└─────────────────────────────────────────────────────────────────────┘
         │              │              │              │
         ▼              ▼              ▼              ▼
┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐
│  Identity   │ │   Catalog   │ │  Ordering   │ │   Basket    │
│   Module    │ │   Module    │ │   Module    │ │   Module    │
├─────────────┤ ├─────────────┤ ├─────────────┤ ├─────────────┤
│ .Core       │ │ .Core       │ │ .Core       │ │ .Core       │
│ .Infra      │ │ .Infra      │ │ .Infra      │ │ .Infra      │
│ .Contracts  │ │ .Contracts  │ │ .Contracts  │ │ .Contracts  │
└─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘
```

## Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Run Locally

```bash
# Clone repository
git clone https://github.com/yourusername/ArchitecturePlayground.git
cd ArchitecturePlayground

# Start infrastructure (PostgreSQL, MongoDB, Redis, RabbitMQ)
docker-compose up -d

# Run API
dotnet run --project src/Bootstrapper/ArchitecturePlayground.API

# Run Frontend (in another terminal)
cd src/Web/vue-storefront
npm install
npm run dev
```

### API Endpoints

| Endpoint | Description |
|----------|-------------|
| `http://localhost:5000` | API |
| `http://localhost:5000/swagger` | Swagger UI |
| `http://localhost:5173` | Vue Frontend |

## Project Structure

```
ArchitecturePlayground/
├── src/
│   ├── Bootstrapper/              # API Host (Composition Root)
│   ├── Modules/                   # Business Modules
│   │   ├── Identity/              # Authentication & Users
│   │   ├── Catalog/               # Products (MongoDB)
│   │   ├── Ordering/              # Orders (PostgreSQL)
│   │   ├── Basket/                # Shopping Cart (Redis)
│   │   ├── Payment/               # Payment Processing
│   │   └── Notification/          # Email & Push
│   ├── Shared/                    # Shared Kernel
│   └── Web/vue-storefront/        # Vue 3 Frontend
├── tests/                         # Unit, Integration, E2E, Architecture
├── docs/                          # Documentation, ADRs, Diagrams
└── infrastructure/                # Docker, Scripts
```

## Key Patterns Implemented

### Vertical Slice Architecture
Each feature is self-contained:
```
Features/CreateOrder/
├── CreateOrderCommand.cs
├── CreateOrderHandler.cs
├── CreateOrderValidator.cs
└── CreateOrderEndpoint.cs
```

### CQRS + MediatR
```csharp
// Command
public record CreateOrderCommand(Guid UserId, List<OrderItem> Items) : ICommand<Result<Guid>>;

// Query
public record GetOrderQuery(Guid OrderId) : IQuery<OrderDto?>;
```

### Outbox Pattern for Reliable Messaging
```csharp
// Atomic: Save Order + Outbox Message in same transaction
await _dbContext.SaveChangesAsync();  // Includes OutboxMessage

// Background worker publishes to RabbitMQ
```

### Process Manager (Order Workflow)
```
Created → InventoryReserved → Paid → Shipped → Delivered
    ↓            ↓
 Cancelled   Cancelled (payment failed)
```

## Testing

```bash
# Run all tests
dotnet test

# Run specific category
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration
dotnet test --filter Category=Architecture
```

## Documentation

- [Architecture Plan](docs/ARCHITECTURE_PLAN.md) - Full technical specification
- [Architecture Decisions](docs/adr/) - ADRs
- [API Documentation](docs/api/) - OpenAPI specs
- [Diagrams](docs/diagrams/) - C4, sequence, ERD

## Cloud Services (Free Tiers)

| Service | Provider | Purpose |
|---------|----------|---------|
| PostgreSQL | Supabase | Main DB |
| MongoDB | Atlas | Catalog |
| Redis | Upstash | Cache, Sessions |
| RabbitMQ | CloudAMQP | Messaging |
| Secrets | Azure Key Vault | Secrets |
| Storage | Azure Blob | Files |
| Monitoring | Azure App Insights | APM |

## Development Approach

- **TDD** for Domain and Application layers
- **Architecture Tests** to enforce module boundaries
- **Conventional Commits** for clear history

## License

MIT License - see [LICENSE](LICENSE) for details.

---

