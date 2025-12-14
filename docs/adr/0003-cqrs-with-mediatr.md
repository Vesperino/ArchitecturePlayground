# ADR-0003: CQRS with MediatR

## Status

Implemented (Interfaces)

## Date

2025-01 (Updated: 2025-12-14)

## Context

We need a pattern for handling commands (write operations) and queries (read operations) in our application. The pattern should:
- Decouple request handling from HTTP layer
- Support cross-cutting concerns (validation, logging, transactions)
- Enable easy testing
- Work well with Vertical Slice Architecture

## Decision

We will use **CQRS (Command Query Responsibility Segregation)** pattern implemented with **MediatR** library.

```csharp
// Command (write)
public record CreateOrderCommand(Guid UserId, List<OrderItem> Items)
    : ICommand<Result<Guid>>;

// Query (read)
public record GetOrderQuery(Guid OrderId)
    : IQuery<OrderDto?>;

// Handler
public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        // Business logic
    }
}
```

MediatR Behaviors for cross-cutting concerns:
- `ValidationBehavior` - FluentValidation
- `LoggingBehavior` - Request/Response logging
- `TransactionBehavior` - Unit of Work

## Consequences

### Positive

- **Separation of concerns**: Commands and queries are independent
- **Testability**: Handlers are easy to unit test
- **Extensibility**: Behaviors for cross-cutting concerns
- **Decoupling**: Endpoints don't depend on handler implementations
- **Single Responsibility**: Each handler does one thing

### Negative

- **Indirection**: Extra layer between endpoint and logic
- **Boilerplate**: Command, Handler, Validator per feature
- **Learning curve**: MediatR concepts to understand

### Neutral

- Not full CQRS with separate read/write databases (yet)
- Can evolve to Event Sourcing later

## Alternatives Considered

### Alternative 1: Direct Service Injection

Inject services directly into controllers/endpoints.

**Rejected because:**
- Harder to add cross-cutting concerns
- Services tend to grow into God objects
- Less testable

### Alternative 2: Custom Mediator Implementation

Build our own mediator without MediatR.

**Rejected because:**
- Reinventing the wheel
- MediatR is battle-tested
- Good community support

## Implementation Status

**Implemented in:** `ArchitecturePlayground.Common.Abstractions.CQRS` (2025-12-14)

CQRS interfaces are now part of the Shared Kernel:

```csharp
// Commands
public interface ICommand : IRequest;
public interface ICommand<out TResponse> : IRequest<TResponse>;

// Queries
public interface IQuery<out TResponse> : IRequest<TResponse>;

// Handlers
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
```

All interfaces extend MediatR's `IRequest` and `IRequestHandler` for seamless pipeline integration.

**Next steps:**
- Implement MediatR behaviors (ValidationBehavior, LoggingBehavior, TransactionBehavior) in `Common.Infrastructure`
- Use interfaces in module features (Identity, Catalog, Orders)

See `docs/plans/2025-12-14-shared-kernel-design.md` for detailed implementation documentation.

## References

- [MediatR documentation](https://github.com/jbogard/MediatR)
- [CQRS Pattern - Microsoft](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
