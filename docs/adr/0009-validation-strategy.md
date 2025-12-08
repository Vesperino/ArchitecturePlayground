# ADR-0009: Validation Strategy

## Status

Accepted

## Date

2025-01

## Context

The application needs comprehensive validation at multiple layers:
- Input validation (API requests)
- Business rule validation (domain logic)
- Data integrity validation (database constraints)

Without a clear strategy, validation logic becomes scattered and duplicated across the codebase, leading to:
- Inconsistent error messages
- Missing validations
- Difficult to test
- Poor user experience

## Decision

We will implement **layered validation** with two distinct types:

### 1. Input Validation (FluentValidation)

Used in Application layer for Commands and Queries.

**Location:** `{Feature}Validator.cs` next to Handler

**Validates:**
- Required fields
- Format (email, phone, URL)
- Length constraints
- Range constraints
- Cross-field validation

**Example:**
```csharp
public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty()
            .WithMessage("Order must contain at least one item");
        RuleFor(x => x.Items).Must(items => items.Count <= 100)
            .WithMessage("Cannot order more than 100 items");
    }
}
```

**Execution:** Via MediatR `ValidationBehavior` pipeline before handler

### 2. Domain Validation

Used in Domain layer within entities and value objects.

**Location:** Inside aggregates, value objects, domain services

**Validates:**
- Invariants (business rules that must always be true)
- State transitions
- Aggregate consistency

**Example:**
```csharp
public sealed class Order : AggregateRoot
{
    public Result Cancel()
    {
        if (Status == OrderStatus.Completed)
            return Result.Failure("Cannot cancel completed order");

        if (Status == OrderStatus.Shipped)
            return Result.Failure("Cannot cancel shipped order");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Id));
        return Result.Success();
    }
}
```

## Consequences

### Positive

- **Clear separation**: Input validation vs business rules
- **Fail fast**: Invalid requests rejected before reaching domain
- **Testable**: Each validator is independently testable
- **Consistent errors**: Standardized error format
- **Type safety**: Compile-time validation for FluentValidation rules
- **Rich error messages**: Detailed validation errors for clients

### Negative

- **Learning curve**: Team needs to learn FluentValidation
- **Slight overhead**: Validation pipeline adds milliseconds to request
- **Duplication**: Some validations appear in both layers (by design)

### Neutral

- Validation runs on every request (consider caching for expensive validations)
- Error format standardized via Problem Details (RFC 7807)

## Validation Flow

```
HTTP Request
    ↓
Input Validation (FluentValidation)
    ↓ (if valid)
Domain Logic
    ↓
Domain Validation (invariants)
    ↓ (if valid)
Persist
```

## Error Response Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["Email is required", "Email format is invalid"],
    "Items": ["Order must contain at least one item"]
  }
}
```

## Alternatives Considered

### Alternative 1: Data Annotations only

Use `[Required]`, `[EmailAddress]` attributes on DTOs.

**Rejected because:**
- Limited expressiveness (can't do complex cross-field validation)
- Couples models to validation framework
- Difficult to test
- Poor error messages out of the box

### Alternative 2: Manual validation in handlers

Check conditions with if statements in handlers.

**Rejected because:**
- Validation logic scattered across codebase
- Difficult to reuse
- Inconsistent error handling
- Hard to test in isolation

### Alternative 3: Validate everything in domain

All validation in domain entities.

**Rejected because:**
- Domain polluted with format validations (email regex, etc.)
- Can't fail fast (invalid requests reach domain layer)
- Mixing infrastructure concerns (input format) with domain (business rules)

## References

- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [RFC 7807 - Problem Details](https://datatracker.ietf.org/doc/html/rfc7807)
- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Validation in DDD - Vladimir Khorikov](https://enterprisecraftsmanship.com/posts/validation-in-ddd/)
