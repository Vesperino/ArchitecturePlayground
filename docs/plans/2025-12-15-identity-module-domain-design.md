# Identity Module Domain Design

**Date:** 2025-12-15
**Status:** Implemented
**Style:** CodeOpinion (pragmatic DDD)
**Tests:** 46 (Email: 25, Password: 7, User: 13, other: 1)

## Architectural Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Approach | Pure DDD | No ASP.NET Core Identity - simpler testing, consistency with architecture |
| UserId | `Guid` directly | YAGNI - Strongly-Typed ID is overengineering for this project |
| RefreshToken | Infrastructure (POCO) | Not a domain concept - no business rules |
| Input validation | FluentValidation | firstName/lastName validation in RegisterCommandValidator |
| Login audit | Infrastructure | Details (IP, User-Agent) are not domain concerns |

## File Structure

```
Identity.Core/
├── Domain/
│   ├── User.cs                         # Aggregate Root
│   ├── ValueObjects/
│   │   ├── Email.cs                    # RFC 5322 validation + normalization
│   │   └── Password.cs                 # Hash only (IPasswordHasher in Infrastructure)
│   └── Events/
│       ├── UserCreatedEvent.cs         # → Notification module (welcome email)
│       └── UserPasswordChangedEvent.cs # → Security alert
└── Features/
    └── (Register, Login, etc. - next step)
```

## Implementation

### Email Value Object

```csharp
public sealed class Email : ValueObject
{
    public const int MaxLength = 254;
    private const int MaxLocalPartLength = 64;
    private const int MaxDomainLength = 255;

    // RFC 5322 compliant pattern
    private static readonly Regex EmailRegex = new(
        @"^(?!.*\.\.)(?!\.)[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]{1,64}(?<!\.)@(?!-)[a-zA-Z0-9-]{1,63}(?<!-)(\.[a-zA-Z0-9-]{1,63}(?<!-))+$",
        RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return DomainError.Validation("Email.Empty", "Email is required.");

        var normalized = email.Trim().ToLowerInvariant();

        if (normalized.Length > MaxLength)
            return DomainError.Validation("Email.TooLong",
                $"Email cannot exceed {MaxLength} characters.");

        if (!EmailRegex.IsMatch(normalized))
            return DomainError.Validation("Email.InvalidFormat",
                "Email format is invalid.");

        return new Email(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
```

**RFC 5322 Validation:**
- No double dots (`..`)
- Does not start/end with a dot
- Domain does not start/end with `-`
- Segment length limits (local: 64, domain segments: 63)
- Normalization: `Trim()` + `ToLowerInvariant()`

### Password Value Object

```csharp
public sealed class Password : ValueObject
{
    public string Hash { get; }

    private Password(string hash) => Hash = hash;

    /// <summary>
    /// Creates Password from already hashed value.
    /// Use IPasswordHasher in Infrastructure to create the hash.
    /// </summary>
    public static Result<Password> FromHash(string? hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return DomainError.Validation("Password.Empty", "Password hash is required.");

        return new Password(hash);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Hash;
    }

    // No ToString() - security by design
}
```

**Hashing Flow:**
```
RegisterCommand (plain-text)
    ↓
RegisterCommandValidator (FluentValidation - complexity rules)
    ↓
RegisterHandler:
    hash = _passwordHasher.Hash(command.Password)
    password = Password.FromHash(hash)
```

### Domain Events

```csharp
// UserCreatedEvent.cs
public sealed class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }

    public UserCreatedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

// UserPasswordChangedEvent.cs
public sealed class UserPasswordChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public DateTime ChangedAt { get; }

    public UserPasswordChangedEvent(Guid userId, DateTime changedAt)
    {
        UserId = userId;
        ChangedAt = changedAt;
    }
}
```

### User Aggregate Root

```csharp
public sealed class User : AggregateRoot<Guid>
{
    public Email Email { get; private set; }
    public Password Password { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public HashSet<string> Roles { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private User(
        Guid id,
        Email email,
        Password password,
        string firstName,
        string lastName) : base(id)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        Roles = [];
        EmailConfirmed = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(
        Email email,
        Password password,
        string firstName,
        string lastName)
    {
        var user = new User(Guid.NewGuid(), email, password, firstName, lastName);
        user.AddDomainEvent(new UserCreatedEvent(user.Id, email.Value));
        return user;
    }

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword;
        AddDomainEvent(new UserPasswordChangedEvent(Id, DateTime.UtcNow));
    }

    public void ConfirmEmail() => EmailConfirmed = true;

    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;

    public void AddRole(string role) => Roles.Add(role);

    public void RemoveRole(string role) => Roles.Remove(role);

    private User() : base(default) { } // EF Core
}
```

## TODO: Infrastructure (to be implemented later)

### LoginAuditLog - Security Audit

```csharp
// Identity.Infrastructure/Persistence/LoginAuditLog.cs
public class LoginAuditLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string? Location { get; set; }      // GeoIP lookup
    public string? DeviceInfo { get; set; }    // Parsed from User-Agent
    public bool Success { get; set; }
    public string? FailureReason { get; set; } // "InvalidPassword", "AccountLocked", etc.
    public DateTime OccurredAt { get; set; }
}
```

**Use cases:**
- "Login detected from new device" - email notification
- "Login history" - view in user profile
- "Suspicious activity" - multiple failed attempts from different IPs

### RefreshToken - Token Management

```csharp
// Identity.Infrastructure/Persistence/RefreshToken.cs
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; }      // Never plain-text
    public DateTime ExpiresAt { get; set; }    // 7 days from creation
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByTokenId { get; set; } // Token rotation

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}
```

### IPasswordHasher - Interface

```csharp
// Identity.Core/Abstractions/IPasswordHasher.cs (or Common.Abstractions)
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
```

**Implementation in Infrastructure** - e.g., BCrypt, Argon2, or ASP.NET Core Identity's PasswordHasher.

## Next Steps

1. ~~**Domain Implementation** (this plan) - TDD~~ Implemented 2025-12-15
2. **Features: Register** - Command, Handler, Validator, Endpoint
3. **Features: Login** - with LoginAuditLog
4. **Infrastructure: Persistence** - EF Core, DbContext
5. **Infrastructure: Services** - PasswordHasher, JwtTokenService

## Related Documents

- [ADR-0009: Validation Strategy](../adr/0009-validation-strategy.md)
- [ADR-0010: Error Handling Strategy](../adr/0010-error-handling-strategy.md)
- [ADR-0011: Pure DDD for Identity Module](../adr/0011-pure-ddd-identity-module.md)
- [Shared Kernel Design](./2025-12-14-shared-kernel-design.md)
