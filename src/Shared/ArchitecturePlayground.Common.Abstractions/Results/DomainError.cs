namespace ArchitecturePlayground.Common.Abstractions.Results;

/// <summary>
/// Represents an error with code, message, and type for HTTP mapping.
/// </summary>
public sealed record DomainError
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    public DomainError(string code, string message, ErrorType type = ErrorType.Failure)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public static readonly DomainError None = new(string.Empty, string.Empty);

    public static DomainError Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    public static DomainError Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static DomainError NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static DomainError Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static DomainError Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    public static DomainError Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);
}
