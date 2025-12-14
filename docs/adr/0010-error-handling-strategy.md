# ADR-0010: Error Handling Strategy

## Status

Implemented

## Date

2025-01 (Updated: 2025-12-14)

## Context

The application needs consistent error handling that:
- Provides clear error messages to clients
- Doesn't leak implementation details
- Follows industry standards
- Distinguishes between expected and unexpected errors
- Provides debugging information for developers

Traditional exception-based flow control has problems:
- Exceptions are expensive (stack traces)
- Control flow is implicit (hidden goto)
- Difficult to type-check what errors a method can return
- Mix of expected errors (validation) and unexpected errors (null reference)

## Decision

We will use a **hybrid approach**:

### 1. Result Pattern for Expected Errors

Use `Result<T>` type for operations that can fail in expected ways.

**Use for:**
- Validation failures
- Business rule violations
- Not found scenarios
- Authorization failures

**Example:**
```csharp
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public DomainError Error { get; }

    public static Result<T> Success(T value);
    public static Result<T> Failure(DomainError error);
}

// Usage
public async Task<Result<Guid>> Handle(CreateOrderCommand cmd, CancellationToken ct)
{
    if (!await _inventory.HasStock(cmd.Items))
        return DomainError.Validation("Order.InsufficientStock", "Insufficient stock");

    var order = Order.Create(cmd.UserId, cmd.Items);
    await _repository.AddAsync(order);

    return order.Id; // Implicit conversion from T to Result<T>
}
```

### 2. Exceptions for Unexpected Errors

Use exceptions for genuinely exceptional situations.

**Use for:**
- Database connection failures
- Network timeouts
- Null reference (programming errors)
- Out of memory

**Handled by:** Global exception middleware

### 3. Problem Details (RFC 7807) for HTTP Responses

All errors returned to clients use Problem Details format.

**Example response:**
```json
{
  "type": "https://example.com/errors/insufficient-stock",
  "title": "Insufficient Stock",
  "status": 400,
  "detail": "Product 'iPhone 15' is out of stock",
  "instance": "/api/v1/orders",
  "traceId": "0HN1234567890"
}
```

## Error Handling Flow

```
Handler returns Result.Failure
    ↓
Endpoint maps Result to IResult
    ↓
IResult generates Problem Details
    ↓
HTTP Response (400/404/409/etc)


Exception thrown
    ↓
Global Exception Middleware
    ↓
Log exception (with trace ID)
    ↓
Return Problem Details (500)
```

## Consequences

### Positive

- **Explicit error handling**: Compiler forces you to handle `Result`
- **Type safety**: Errors are part of method signature
- **Performance**: Result pattern faster than exceptions for expected errors
- **Clear intent**: Result = expected, Exception = unexpected
- **Standardized format**: RFC 7807 is industry standard
- **Debuggable**: Trace IDs link logs to requests

### Negative

- **Learning curve**: Team needs to learn Result pattern
- **Verbosity**: More code than throwing exceptions
- **Inconsistency risk**: Mix of Result and exceptions

### Neutral

- Need to ensure all exceptions are caught by middleware
- Logging must include correlation IDs for distributed tracing
- Development environment can return stack traces, production must not

## Error Types

```csharp
public sealed record DomainError
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    public static DomainError NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static DomainError Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static DomainError Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static DomainError Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    public static DomainError Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);
}

public enum ErrorType
{
    Failure,      // 500
    Validation,   // 400
    NotFound,     // 404
    Conflict,     // 409
    Unauthorized, // 401
    Forbidden     // 403
}
```

## HTTP Status Code Mapping

| Error Type | Status Code | Use Case |
|------------|-------------|----------|
| Validation | 400 | Invalid input |
| Unauthorized | 401 | Not authenticated |
| Forbidden | 403 | Not authorized |
| NotFound | 404 | Resource doesn't exist |
| Conflict | 409 | Business rule violation |
| Exception | 500 | Unexpected error |

## Alternatives Considered

### Alternative 1: Exceptions for everything

Use exceptions for both expected and unexpected errors.

**Rejected because:**
- Expensive for expected errors (validation happens frequently)
- Control flow via exceptions is anti-pattern
- Difficult to track what errors a method can return

### Alternative 2: Error codes (int return values)

Return -1, 0, 1 for error/success like C code.

**Rejected because:**
- Not type-safe
- No error messages
- Outdated pattern
- Doesn't work with async/await

### Alternative 3: OneOf discriminated unions

Use `OneOf<Success, Error>` library.

**Rejected because:**
- Additional dependency
- Result pattern is simpler for our use case
- Less familiar to .NET developers

## Implementation Status

**Implemented in:** `ArchitecturePlayground.Common.Abstractions` (2025-12-14)

- `Result` and `Result<T>` in `Results/Result.cs` and `Results/ResultT.cs`
- `DomainError` in `Results/DomainError.cs`
- `ErrorType` enum in `Results/ErrorType.cs`
- Domain exceptions in `Exceptions/` (DomainException, NotFoundException, ConflictException, ValidationException)
- **42 unit tests** covering all functionality

**Design choices made:**
- Named `DomainError` instead of `Error` to avoid naming conflicts with system types
- Used simple Result style (not Railway Oriented Programming) for pragmatism
- Included implicit conversions (`T -> Result<T>`, `DomainError -> Result`) to reduce boilerplate
- Provided `Match` method for functional-style pattern matching when needed

See `docs/plans/2025-12-14-shared-kernel-design.md` for detailed implementation documentation.

## References

- [RFC 7807 - Problem Details for HTTP APIs](https://datatracker.ietf.org/doc/html/rfc7807)
- [Vladimir Khorikov - Functional C#](https://enterprisecraftsmanship.com/posts/functional-c-handling-failures-input-errors/)
- [Martin Fowler - Notification Pattern](https://martinfowler.com/eaaDev/Notification.html)
- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/)
