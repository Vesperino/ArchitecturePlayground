namespace ArchitecturePlayground.Common.Abstractions.Results;

/// <summary>
/// Represents the type of error for HTTP status code mapping.
/// </summary>
public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5
}
