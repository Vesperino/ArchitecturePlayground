# ADR-0002: Vertical Slice Architecture

## Status

Accepted

## Date

2025-01

## Context

Within each module, we need to decide how to organize code. Traditional Clean Architecture separates code by technical concern (Controllers, Services, Repositories), which can lead to:
- Scattered feature code across many folders
- High coupling between layers
- Difficulty understanding a feature's full implementation

## Decision

We will use **Vertical Slice Architecture** within each module.

Each feature is self-contained in a single folder:
```
Features/
└── CreateOrder/
    ├── CreateOrderCommand.cs      # Request DTO
    ├── CreateOrderHandler.cs      # Business logic
    ├── CreateOrderValidator.cs    # Validation rules
    └── CreateOrderEndpoint.cs     # HTTP endpoint
```

Combined with Clean Architecture principles:
- Domain folder for entities and value objects
- Clear separation of concerns within each slice

## Consequences

### Positive

- **Feature cohesion**: All code for a feature in one place
- **Easy to understand**: New developers can focus on one folder
- **Independent features**: Changes to one feature don't affect others
- **Simple testing**: Test entire feature in isolation
- **Natural CQRS**: Commands and queries are separate slices

### Negative

- **Potential duplication**: Similar code across slices
- **Learning curve**: Different from traditional layered approach
- **Shared code decisions**: Must decide what goes in Domain vs Feature

### Neutral

- Works well with MediatR pattern
- Each slice can use different approaches if needed

## Alternatives Considered

### Alternative 1: Traditional Clean Architecture Layers

Separate folders for Application, Domain, Infrastructure within each module.

**Rejected because:**
- Too much ceremony for simple features
- Feature code scattered across folders
- Harder to see full feature implementation

### Alternative 2: Feature Folders with Shared Services

Feature folders but with shared service layer.

**Rejected because:**
- Services become God objects
- Unclear boundaries between features

## References

- [Vertical Slice Architecture - Jimmy Bogard](https://www.jimmybogard.com/vertical-slice-architecture/)
- [VSA in .NET - Milan Jovanovic](https://www.milanjovanovic.tech/blog/vertical-slice-architecture)
