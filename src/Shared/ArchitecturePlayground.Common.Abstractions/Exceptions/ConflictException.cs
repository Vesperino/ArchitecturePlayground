namespace ArchitecturePlayground.Common.Abstractions.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated.
/// </summary>
public sealed class ConflictException : DomainException
{
    public ConflictException(string message) : base(message)
    {
    }
}
