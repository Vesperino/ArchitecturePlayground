# Shared Kernel Design

**Date:** 2025-12-14
**Status:** Implemented

## Overview

Implementation of the Shared Kernel (Common.Abstractions) for the e-commerce modular monolith.

## Components Implemented

### 1. Result Pattern (`Results/`)

| Class | Purpose |
|-------|---------|
| `DomainError` | Immutable error with Code, Message, Type for HTTP mapping |
| `ErrorType` | Enum for HTTP status mapping (Validation, NotFound, Conflict, etc.) |
| `Result` | Non-generic result for void operations |
| `Result<T>` | Generic result with value and implicit conversions |

**Design decisions:**
- Simple style (no Railway/FP) - aligned with CodeOpinion pragmatic approach
- Implicit conversions for less boilerplate
- Optional `Match` for pattern matching scenarios
- No external dependencies (owns the abstraction)

### 2. Domain Building Blocks (`Domain/`)

| Class | Purpose |
|-------|---------|
| `Entity<TId>` | Base class with typed ID and equality by ID |
| `AggregateRoot<TId>` | Entity + domain event collection |
| `ValueObject` | Equality by properties |
| `IDomainEvent` | Marker interface with Id and OccurredOn |
| `DomainEvent` | Base implementation with auto-generated Id/timestamp |

### 3. CQRS Interfaces (`CQRS/`)

| Interface | Purpose |
|-----------|---------|
| `ICommand` | Marker for commands (no return) |
| `ICommand<T>` | Marker for commands with return value |
| `IQuery<T>` | Marker for queries |
| `ICommandHandler<T>` | Handler for void commands |
| `ICommandHandler<T,R>` | Handler for commands with return |
| `IQueryHandler<T,R>` | Handler for queries |

All extend MediatR interfaces for pipeline integration.

### 4. Domain Exceptions (`Exceptions/`)

| Exception | HTTP Status | Use Case |
|-----------|-------------|----------|
| `DomainException` | 500 | Base for domain errors |
| `NotFoundException` | 404 | Resource not found |
| `ConflictException` | 409 | Business rule violation |
| `ValidationException` | 400 | Domain validation failure |

## Usage Examples

### Result Pattern
```csharp
public Result<Order> CreateOrder(CreateOrderCommand cmd)
{
    if (!valid)
        return DomainError.Validation("Order.Invalid", "Cart is empty");

    return new Order(cmd.UserId);  // implicit conversion
}
```

### Aggregate with Domain Events
```csharp
public sealed class Order : AggregateRoot<Guid>
{
    public Order(Guid userId) : base(Guid.NewGuid())
    {
        AddDomainEvent(new OrderCreatedEvent(Id, userId));
    }
}
```

### Value Object
```csharp
public sealed class Email : ValueObject
{
    public string Value { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

### CQRS Command
```csharp
public record CreateOrderCommand(Guid UserId) : ICommand<Result<Guid>>;

public sealed class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand cmd, CancellationToken ct)
    {
        // ...
    }
}
```

## Test Coverage

- **42 unit tests** covering all components
- Tests in `tests/Shared/Common.Abstractions.Tests/`

## Dependencies

- `MediatR` (12.4.1) - for CQRS interface inheritance

## File Structure

```
src/Shared/ArchitecturePlayground.Common.Abstractions/
├── CQRS/
│   ├── ICommand.cs
│   ├── ICommandHandler.cs
│   ├── IQuery.cs
│   └── IQueryHandler.cs
├── Domain/
│   ├── AggregateRoot.cs
│   ├── DomainEvent.cs
│   ├── Entity.cs
│   ├── IDomainEvent.cs
│   └── ValueObject.cs
├── Exceptions/
│   ├── ConflictException.cs
│   ├── DomainException.cs
│   ├── NotFoundException.cs
│   └── ValidationException.cs
└── Results/
    ├── DomainError.cs
    ├── ErrorType.cs
    ├── Result.cs
    └── ResultT.cs
```

## Next Steps

1. Implement Identity Module domain (User aggregate, Email/Password value objects)
2. Add ValidationBehavior to Common.Infrastructure
3. Implement persistence layer
