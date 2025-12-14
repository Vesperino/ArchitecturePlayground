using ArchitecturePlayground.Common.Abstractions.Results;
using FluentAssertions;

namespace ArchitecturePlayground.Common.Abstractions.Tests.Results;

public sealed class ResultTTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResultWithValue()
    {
        // Arrange
        var value = 42;

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(42);
        result.Error.Should().Be(DomainError.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResultWithError()
    {
        // Arrange
        var error = DomainError.Validation("Test.Error", "Test error message");

        // Act
        var result = Result.Failure<int>(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ValueWhenFailure_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var error = DomainError.Validation("Test.Error", "Test error message");
        var result = Result.Failure<int>(error);

        // Act
        var act = () => result.Value;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*failure*");
    }

    [Fact]
    public void ImplicitConversionFromValue_ShouldCreateSuccessResult()
    {
        // Arrange
        var value = "test value";

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("test value");
    }

    [Fact]
    public void ImplicitConversionFromError_ShouldCreateFailedResult()
    {
        // Arrange
        var error = DomainError.Validation("Test.Error", "Test error message");

        // Act
        Result<string> result = error;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void MatchWhenSuccess_ShouldExecuteOnSuccessFunc()
    {
        // Arrange
        var result = Result.Success(42);

        // Act
        var output = result.Match(
            onSuccess: value => $"Value: {value}",
            onFailure: error => $"Error: {error.Message}");

        // Assert
        output.Should().Be("Value: 42");
    }

    [Fact]
    public void MatchWhenFailure_ShouldExecuteOnFailureFunc()
    {
        // Arrange
        var error = DomainError.Validation("Test.Error", "Something failed");
        var result = Result.Failure<int>(error);

        // Act
        var output = result.Match(
            onSuccess: value => $"Value: {value}",
            onFailure: err => $"Error: {err.Message}");

        // Assert
        output.Should().Be("Error: Something failed");
    }

    [Fact]
    public void SuccessWithNullValue_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => Result.Success<string>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FailureWithNoneError_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Result.Failure<int>(DomainError.None);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot*None*");
    }
}
