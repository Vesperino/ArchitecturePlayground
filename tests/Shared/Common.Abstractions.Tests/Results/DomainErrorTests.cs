using ArchitecturePlayground.Common.Abstractions.Results;
using FluentAssertions;

namespace ArchitecturePlayground.Common.Abstractions.Tests.Results;

public sealed class DomainErrorTests
{
    [Fact]
    public void ShouldStoreCodeAndMessage()
    {
        // Arrange & Act
        var error = new DomainError("User.NotFound", "User was not found");

        // Assert
        error.Code.Should().Be("User.NotFound");
        error.Message.Should().Be("User was not found");
    }

    [Fact]
    public void None_ShouldReturnEmptyError()
    {
        // Act
        var error = DomainError.None;

        // Assert
        error.Code.Should().BeEmpty();
        error.Message.Should().BeEmpty();
    }

    [Fact]
    public void NotFound_ShouldCreateNotFoundError()
    {
        // Act
        var error = DomainError.NotFound("User.NotFound", "User was not found");

        // Assert
        error.Code.Should().Be("User.NotFound");
        error.Message.Should().Be("User was not found");
        error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void Validation_ShouldCreateValidationError()
    {
        // Act
        var error = DomainError.Validation("Email.Invalid", "Email format is invalid");

        // Assert
        error.Code.Should().Be("Email.Invalid");
        error.Message.Should().Be("Email format is invalid");
        error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Conflict_ShouldCreateConflictError()
    {
        // Act
        var error = DomainError.Conflict("User.AlreadyExists", "User already exists");

        // Assert
        error.Code.Should().Be("User.AlreadyExists");
        error.Message.Should().Be("User already exists");
        error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void Failure_ShouldCreateGenericFailureError()
    {
        // Act
        var error = DomainError.Failure("Operation.Failed", "Something went wrong");

        // Assert
        error.Code.Should().Be("Operation.Failed");
        error.Message.Should().Be("Something went wrong");
        error.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public void ShouldBeEqualWhenCodeAndMessageMatch()
    {
        // Arrange
        var error1 = new DomainError("User.NotFound", "User was not found");
        var error2 = new DomainError("User.NotFound", "User was not found");

        // Assert
        error1.Should().Be(error2);
    }

    [Fact]
    public void ShouldNotBeEqualWhenCodeDiffers()
    {
        // Arrange
        var error1 = new DomainError("User.NotFound", "User was not found");
        var error2 = new DomainError("User.Invalid", "User was not found");

        // Assert
        error1.Should().NotBe(error2);
    }
}
