# ADR-0003: CQRS with MediatR

## Status

Accepted

## Date

2025-01

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

## References

- [MediatR documentation](https://github.com/jbogard/MediatR)
- [CQRS Pattern - Microsoft](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
