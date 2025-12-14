namespace ArchitecturePlayground.Common.Abstractions.Results;

/// <summary>
/// Represents the result of an operation that doesn't return a value.
/// </summary>
public sealed class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public DomainError Error { get; }

    private Result(bool isSuccess, DomainError error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, DomainError.None);

    public static Result Failure(DomainError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (error == DomainError.None)
        {
            throw new ArgumentException("Error cannot be None for a failure result.", nameof(error));
        }

        return new(false, error);
    }

    public static Result<T> Success<T>(T value) => Result<T>.CreateSuccess(value);

    public static Result<T> Failure<T>(DomainError error) => Result<T>.CreateFailure(error);

    public static implicit operator Result(DomainError error) => Failure(error);
}
