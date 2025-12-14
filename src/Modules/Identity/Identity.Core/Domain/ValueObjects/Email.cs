using System.Text.RegularExpressions;
using ArchitecturePlayground.Common.Abstractions.Domain;
using ArchitecturePlayground.Common.Abstractions.Results;

namespace ArchitecturePlayground.Modules.Identity.Core.Domain.ValueObjects;

public sealed partial class Email : ValueObject
{
    public const int MaxLength = 254;

    // RFC 5322 compliant pattern
    [GeneratedRegex(
        @"^(?!.*\.\.)(?!\.)[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]{1,64}(?<!\.)@(?!-)[a-zA-Z0-9-]{1,63}(?<!-)(\.[a-zA-Z0-9-]{1,63}(?<!-))+$",
        RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

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

        if (!EmailRegex().IsMatch(normalized))
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
