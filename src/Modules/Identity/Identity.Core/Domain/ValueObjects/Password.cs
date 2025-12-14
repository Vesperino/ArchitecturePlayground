using ArchitecturePlayground.Common.Abstractions.Domain;
using ArchitecturePlayground.Common.Abstractions.Results;

namespace ArchitecturePlayground.Modules.Identity.Core.Domain.ValueObjects;

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

    // No ToString() - security by design (don't log hash accidentally)
}
