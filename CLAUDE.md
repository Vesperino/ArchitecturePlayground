# CLAUDE.md - ArchitecturePlayground

## Project Overview
E-Commerce platform demonstrating senior-level .NET 9 + Vue 3 skills.
**Architecture:** Modular Monolith + Vertical Slice Architecture (2025 trend)

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | .NET 9, ASP.NET Core Minimal APIs |
| Frontend | Vue 3 (Composition API), TypeScript, Pinia, TailwindCSS |
| Databases | PostgreSQL (Supabase), MongoDB (Atlas), Redis (Upstash) |
| Messaging | MediatR (sync) + MassTransit/RabbitMQ (async via CloudAMQP) |
| ORM | Entity Framework Core 9, Dapper (reports) |

---

## Architecture Principles

### Modular Monolith (NOT Microservices)
- Single deployment unit
- Modules communicate via **Contracts** only
- In-process events (MediatR) + async events (RabbitMQ)
- Can migrate to microservices later if needed

### Vertical Slice Architecture
Each feature is self-contained:
```
Features/
└── CreateOrder/
    ├── CreateOrderCommand.cs
    ├── CreateOrderHandler.cs
    ├── CreateOrderValidator.cs
    └── CreateOrderEndpoint.cs
```

### Module Structure (3 projects per module)
```
Module/
├── Module.Core/           # Domain + Application + Features
├── Module.Infrastructure/ # Persistence, External Services
└── Module.Contracts/      # Public API for other modules
```

---

## Key Patterns

| Pattern | Usage |
|---------|-------|
| CQRS | MediatR Commands/Queries |
| Outbox | Atomic DB + Event publishing |
| Process Manager | Order workflow state machine |
| Result Pattern | No exceptions for expected failures |
| Repository | Abstraction over EF Core |

---

## Development Approach

### TDD
- Write tests FIRST for Domain + Application logic
- Unit tests: Domain entities, Handlers
- Integration tests: Endpoints with Testcontainers
- Architecture tests: NetArchTest for module boundaries

### Code Style
- Use `sealed` for classes that shouldn't be inherited
- Use `record` for DTOs and Commands
- Async all the way
- CancellationToken in all async methods

---

## Module Boundaries (CRITICAL)

### Allowed References
```
Module.Core → Shared.Abstractions
Module.Core → Module.Contracts (own only)
Module.Core → OtherModule.Contracts (for integration)

Module.Infrastructure → Module.Core
Module.Infrastructure → Shared.Infrastructure
```

### FORBIDDEN References
```
❌ Module.Core → OtherModule.Core
❌ Module.Core → OtherModule.Infrastructure
❌ Module.Infrastructure → OtherModule.Infrastructure
```

---

## Messaging Strategy

### MediatR (Sync)
- Queries (GET requests)
- Simple commands that don't need async processing
- Within single HTTP request

### RabbitMQ/MassTransit (Async)
- Long-running operations
- Cross-module side effects
- External API calls (payment, email)
- Fire-and-forget with durability

---

## Cloud Services (Free Tiers)

| Service | Provider | Purpose |
|---------|----------|---------|
| PostgreSQL | Supabase | Main DB (Identity, Orders) |
| MongoDB | Atlas | Catalog (flexible schema) |
| Redis | Upstash | Cache, Sessions, Saga state |
| RabbitMQ | CloudAMQP | Async messaging |
| Secrets | Azure Key Vault | Secrets management |
| Storage | Azure Blob | Product images |
| Monitoring | Azure App Insights | APM |
| Email | SendGrid | Transactional emails |

---

## Naming Conventions

### Files
- Commands: `{Action}Command.cs` (e.g., `CreateOrderCommand.cs`)
- Handlers: `{Action}Handler.cs`
- Validators: `{Action}Validator.cs`
- Endpoints: `{Action}Endpoint.cs`

### Folders
- `Features/` - Vertical slices
- `Domain/` - Entities, Value Objects, Events
- `Persistence/` - DbContext, Configurations
- `Services/` - External service implementations

---

## Common Commands

```bash
# Run API
dotnet run --project src/Bootstrapper/ArchitecturePlayground.API

# Run tests
dotnet test

# Run with Docker
docker-compose up -d

# EF Migrations
dotnet ef migrations add MigrationName -p src/Modules/Identity/Identity.Infrastructure -s src/Bootstrapper/ArchitecturePlayground.API

# Vue dev
cd src/Web/vue-storefront && npm run dev
```

---

## Important Files

| File | Purpose |
|------|---------|
| `Directory.Build.props` | Shared MSBuild properties |
| `Directory.Packages.props` | Central Package Management |
| `.editorconfig` | Code style rules |
| `docker-compose.yml` | Local development infrastructure |

---

## Security Considerations (OWASP)

- All inputs validated with FluentValidation
- Parameterized queries only (EF Core handles this)
- JWT with short expiry (15min) + refresh token rotation
- Rate limiting on auth endpoints
- CORS configured strictly
- Security headers via middleware
- Secrets in Azure Key Vault (never in code)

---

## When Implementing Features

1. Create feature folder in appropriate module
2. Write Command/Query record
3. Write Validator (FluentValidation)
4. Write Handler with domain logic
5. Write Endpoint (Minimal API)
6. Write unit tests for Handler
7. Write integration tests for Endpoint
8. Update OpenAPI documentation

---

## Outbox Pattern Implementation

```csharp
// CRITICAL: All events MUST go through Outbox for atomicity
public override async Task<int> SaveChangesAsync(CancellationToken ct)
{
    var domainEvents = ChangeTracker.Entries<AggregateRoot>()
        .SelectMany(x => x.Entity.DomainEvents);

    foreach (var @event in domainEvents)
    {
        OutboxMessages.Add(new OutboxMessage
        {
            Type = @event.GetType().Name,
            Content = JsonSerializer.Serialize(@event),
            OccurredOn = DateTime.UtcNow
        });
    }

    return await base.SaveChangesAsync(ct);
}
```

Background worker polls Outbox table and publishes to RabbitMQ.

---

## Process Manager (Order Workflow)

MassTransit State Machine for order flow:
```
Created → InventoryReserved → Paid → Shipped → Delivered
    ↓           ↓               ↓
Cancelled   Cancelled       Cancelled
```

Handles:
- Timeout (15min payment timeout)
- Compensation (release inventory on failure)
- Idempotency

---

## Metryki Sukcesu

- [ ] 90%+ code coverage w domain layer
- [ ] Wszystkie OWASP Top 10 zaadresowane
- [ ] < 200ms response time (P95)
- [ ] Architecture tests passing
- [ ] Zero critical security issues
- [ ] Kompletne diagramy C4
- [ ] Min. 10 ADRs
- [ ] README z badges
- [ ] Postman collection
- [ ] Working CI/CD pipeline

---

## Common Pitfalls - AVOID!

### ❌ DON'T:
- Direct references between module .Core/.Infrastructure
- Anemic Domain Model (logic in handlers instead of aggregates)
- God classes / Big Ball of Mud
- Primitive Obsession (use Value Objects!)
- Exception-based flow control
- Committing secrets
- Testing only happy path
- Over-engineering (YAGNI!)

### ✅ DO:
- Bounded Contexts from DDD
- Rich Domain Model
- Value Objects for business concepts
- Result pattern instead of exceptions for expected errors
- Fail fast validation
- Test edge cases and failure scenarios
- Document decisions (ADRs)
- Keep it simple (pragmatism > perfection)

---

## Questions to Ask User

If something is unclear or ambiguous:
1. Stop
2. Explain the problem
3. Present options with pros/cons
4. Ask for decision
5. Document in ADR

**Don't blindly agree - this should be critical discussion!**

---

## Mandatory Skills Workflow (CRITICAL)

Claude MUST use skills from `superpowers` plugin. This is NOT optional.

### Before ANY Task
1. Check if relevant skill exists
2. If yes → Use the Skill tool to load it
3. Announce which skill is being used
4. Follow skill instructions exactly

### Mandatory Skills by Context

| Context | Required Skill |
|---------|----------------|
| Before writing ANY code | `superpowers:brainstorming` |
| Implementing features/bugfixes | `superpowers:test-driven-development` |
| Debugging/errors | `superpowers:systematic-debugging` |
| Before claiming "done" | `superpowers:verification-before-completion` |
| After implementation + tests pass | `superpowers:post-implementation-docs` (custom) |
| Creating implementation plans | `superpowers:writing-plans` |
| Executing plans | `superpowers:executing-plans` |
| Code review needed | `superpowers:requesting-code-review` |
| Receiving review feedback | `superpowers:receiving-code-review` |
| Finishing branch work | `superpowers:finishing-a-development-branch` |
| 3+ independent problems | `superpowers:dispatching-parallel-agents` |

### Post-Implementation Documentation (Custom Workflow)

After feature implementation when tests pass:
1. Dispatch background agent to update documentation
2. Agent updates: README, API docs, ADRs if needed, diagrams
3. Agent creates PR or commits with doc changes
4. Main work continues without blocking

### Forbidden Rationalizations
These thoughts = STOP and use skill:
- "This is simple, no skill needed" → WRONG
- "I'll be quick" → WRONG
- "Skill is overkill" → WRONG
- "Let me just do this first" → WRONG

---

## Current Phase: Documentation & Setup

Priority order:
1. ✅ CLAUDE.md
2. Documentation (docs/, ADRs, diagrams)
3. Project structure (folders, .csproj files)
4. Dependencies (NuGet packages, npm)
5. Implementation (TDD approach)

**Status**: Creating documentation (README, ADRs, C4 diagrams)
**Last Updated**: 2025-12-08
