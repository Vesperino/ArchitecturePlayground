# Architecture Overview

## Architecture Style

**Modular Monolith + Vertical Slice Architecture**

This project demonstrates a pragmatic approach to building enterprise applications in 2025, avoiding the over-engineering trap of microservices while maintaining clear module boundaries.

## C4 Model Diagrams

We use the C4 model for visualizing architecture:

1. **[Context Diagram](C4-Context.puml)** - System context and external actors
2. **[Container Diagram](C4-Container.puml)** - High-level technology choices
3. **[Component Diagrams](C4-Component-Identity.puml)** - Module internals

## Key Architectural Decisions

See [Architecture Decision Records](../adr/) for detailed reasoning behind:

- Why Modular Monolith over Microservices
- Why Vertical Slice Architecture
- Why CQRS with MediatR
- Why Outbox Pattern for messaging
- Why Hybrid Cloud hosting

## Module Structure

Each business module follows this structure:

```
Module/
├── Module.Core/           # Domain + Application (Features)
│   ├── Features/          # Vertical slices
│   ├── Domain/            # Entities, Value Objects, Events
│   └── Exceptions/        # Domain exceptions
├── Module.Infrastructure/ # External concerns
│   ├── Persistence/       # DbContext, Configurations
│   └── Services/          # External service implementations
└── Module.Contracts/      # Public API
    ├── DTOs/              # Data transfer objects
    └── Events/            # Integration events
```

## Communication Patterns

### Synchronous (Within Request)
- **MediatR** for in-process command/query handling
- Direct method calls within a module

### Asynchronous (Background)
- **MassTransit + RabbitMQ** for cross-module events
- **Outbox Pattern** for reliable message delivery

## Data Ownership

Each module owns its data:

| Module | Database | Technology |
|--------|----------|------------|
| Identity | `identity` schema | PostgreSQL |
| Catalog | `catalog` database | MongoDB |
| Ordering | `ordering` schema | PostgreSQL |
| Basket | N/A | Redis |

## Rendering Diagrams

To render PlantUML diagrams:

1. Install [PlantUML extension](https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml) in VS Code
2. Or use online: https://www.plantuml.com/plantuml/uml/

## Tech Stack

See [tech-stack.md](tech-stack.md) for detailed technology choices.
