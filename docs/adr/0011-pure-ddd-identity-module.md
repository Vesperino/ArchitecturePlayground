# ADR-0011: Pure DDD for Identity Module

## Status

Accepted

## Date

2025-12-15

## Context

The application needs user authentication and authorization. The Identity module must handle:
- User registration and login
- Password management (hashing, validation, change)
- JWT token generation and refresh token rotation
- Role-based access control
- Login audit for security

.NET ecosystem provides ASP.NET Core Identity - a full-featured authentication system with:
- User/Role management
- Password hashing (PBKDF2)
- Two-factor authentication
- Account lockout
- External login providers (OAuth)
- EF Core integration

However, ASP.NET Core Identity has trade-offs:
- Tight coupling to EF Core and specific database schema
- `IdentityUser` base class with many properties we don't need
- Difficult to unit test without database
- Opinionated design that may conflict with DDD patterns
- Learning curve for customization

## Decision

We will implement **Pure DDD approach** for the Identity module:

1. **User as Aggregate Root** - `User : AggregateRoot<Guid>` with custom domain logic
2. **Value Objects** - `Email` (RFC 5322 validation), `Password` (hash wrapper)
3. **Domain Events** - `UserCreatedEvent`, `UserPasswordChangedEvent`
4. **Infrastructure Services** - `IPasswordHasher`, `IJwtTokenService` implemented separately

RefreshToken and LoginAuditLog are **not domain entities** - they are infrastructure concerns managed by services.

### Domain Structure

```
Identity.Core/
├── Domain/
│   ├── User.cs                    # Aggregate Root
│   └── ValueObjects/
│       ├── Email.cs               # RFC 5322 validation + normalization
│       └── Password.cs            # Hash only
└── Events/
    ├── UserCreatedEvent.cs
    └── UserPasswordChangedEvent.cs

Identity.Infrastructure/
├── Persistence/
│   ├── RefreshToken.cs            # POCO, not domain entity
│   └── LoginAuditLog.cs           # Security audit
└── Services/
    ├── PasswordHasher.cs          # BCrypt/Argon2
    └── JwtTokenService.cs         # Token generation
```

## Consequences

### Positive

- **Full control** - Domain models exactly match business requirements
- **Testability** - User aggregate can be unit tested without database
- **Consistency** - Same patterns as Catalog, Ordering modules
- **Simplicity** - No unused features (2FA, lockout) until needed
- **Clean separation** - Infrastructure concerns (tokens, audit) clearly separated
- **YAGNI** - Start simple, add features when required

### Negative

- **More code** - Must implement password hashing, token generation ourselves
- **Security responsibility** - Team must ensure proper implementation
- **No built-in features** - 2FA, lockout, external providers need manual implementation
- **Reinventing wheel** - ASP.NET Identity is battle-tested

### Mitigations

- Use proven libraries for crypto: BCrypt.Net or Microsoft.AspNetCore.Identity.PasswordHasher (just the hasher, not full Identity)
- Follow OWASP guidelines for authentication
- Add security features (lockout, 2FA) when business requires them

## Alternatives Considered

### Alternative 1: Full ASP.NET Core Identity

Inherit from `IdentityUser`, use `UserManager<T>`, `SignInManager<T>`.

**Rejected because:**
- Tight coupling to EF Core complicates testing
- Many unused features add complexity
- Difficult to customize for DDD patterns
- Inconsistent with other modules' architecture

### Alternative 2: Hybrid - ASP.NET Identity in Infrastructure

Domain has `User : AggregateRoot<Guid>`, Infrastructure maps to `IdentityUser`.

**Rejected because:**
- Complex mapping between domain and Identity models
- Two sources of truth for user data
- Still inherits Identity's schema constraints
- Over-engineering for current requirements

### Alternative 3: External Identity Provider (Auth0, Keycloak)

Delegate authentication entirely to external service.

**Rejected because:**
- Adds external dependency and cost
- Overkill for learning project
- Less control over user data
- Can migrate to this later if needed

## Implementation Notes

### Password Hashing

Use `Microsoft.AspNetCore.Identity.PasswordHasher<T>` standalone (without full Identity):

```csharp
public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password) =>
        _hasher.HashPassword(null!, password);

    public bool Verify(string password, string hash) =>
        _hasher.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
}
```

This gives us Microsoft's battle-tested PBKDF2 implementation without coupling to Identity.

### JWT Token Service

Use `Microsoft.AspNetCore.Authentication.JwtBearer` for token validation, custom code for generation:

```csharp
public class JwtTokenService : IJwtTokenService
{
    public string GenerateAccessToken(User user, IEnumerable<string> roles);
    public RefreshToken GenerateRefreshToken(Guid userId, string ipAddress);
}
```

### Security Checklist

- [ ] Password hashing with PBKDF2/BCrypt/Argon2 (iteration count >= 100,000)
- [ ] JWT with RS256 or HS256 (256-bit key minimum)
- [ ] Refresh token rotation (new token on each refresh)
- [ ] Refresh token stored as hash, not plain text
- [ ] Rate limiting on login endpoint
- [ ] Login audit logging (IP, User-Agent)
- [ ] Account lockout after N failed attempts (future)

## References

- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
- [ASP.NET Core Identity Documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [JWT Best Practices](https://datatracker.ietf.org/doc/html/rfc8725)
- [ADR-0006: JWT + Refresh Token Strategy](./0006-jwt-refresh-token-strategy.md)
- [Identity Module Domain Design](../plans/2025-12-15-identity-module-domain-design.md)
