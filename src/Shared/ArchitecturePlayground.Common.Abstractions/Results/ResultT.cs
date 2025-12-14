namespace ArchitecturePlayground.Common.Abstractions.Results;

/// <summary>
/// Represents the result of an operation that returns a value of type T.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed class Result<T>
{
    private readonly T? _value;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public DomainError Error { get; }

    public T Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("Cannot access Value on a failure result.");
            }

            return _value!;
        }
    }

    private Result(bool isSuccess, T? value, DomainError error)
    {
        IsSuccess = isSuccess;
        _value = value;
        Error = error;
    }

    internal static Result<T> CreateSuccess(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(true, value, DomainError.None);
    }

    internal static Result<T> CreateFailure(DomainError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (error == DomainError.None)
        {
            throw new ArgumentException("Error cannot be None for a failure result.", nameof(error));
        }

        return new(false, default, error);
    }

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<DomainError, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }

    public static implicit operator Result<T>(T value) => CreateSuccess(value);

    public static implicit operator Result<T>(DomainError error) => CreateFailure(error);
}
