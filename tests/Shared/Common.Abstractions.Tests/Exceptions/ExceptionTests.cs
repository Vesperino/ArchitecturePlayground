using ArchitecturePlayground.Common.Abstractions.Exceptions;
using FluentAssertions;

namespace ArchitecturePlayground.Common.Abstractions.Tests.Exceptions;

public sealed class ExceptionTests
{
    [Fact]
    public void DomainException_ShouldStoreMessage()
    {
        // Act
        var exception = new DomainException("Test error");

        // Assert
        exception.Message.Should().Be("Test error");
    }

    [Fact]
    public void NotFoundException_ShouldStoreResourceTypeAndId()
    {
        // Act
        var exception = new NotFoundException("User", 42);

        // Assert
        exception.ResourceType.Should().Be("User");
        exception.ResourceId.Should().Be(42);
        exception.Message.Should().Contain("User");
        exception.Message.Should().Contain("42");
    }

    [Fact]
    public void NotFoundExceptionFor_ShouldCreateTypedException()
    {
        // Act
        var exception = NotFoundException.For<TestEntity>(Guid.Empty);

        // Assert
        exception.ResourceType.Should().Be("TestEntity");
        exception.ResourceId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void ConflictException_ShouldStoreMessage()
    {
        // Act
        var exception = new ConflictException("Resource already exists");

        // Assert
        exception.Message.Should().Be("Resource already exists");
    }

    [Fact]
    public void ValidationException_WithMessage_ShouldStoreMessage()
    {
        // Act
        var exception = new ValidationException("Validation failed");

        // Assert
        exception.Message.Should().Be("Validation failed");
        exception.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidationException_WithErrors_ShouldStoreErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            ["Email"] = ["Email is required", "Email format is invalid"],
            ["Password"] = ["Password is too short"]
        };

        // Act
        var exception = new ValidationException(errors);

        // Assert
        exception.Errors.Should().HaveCount(2);
        exception.Errors["Email"].Should().HaveCount(2);
        exception.Errors["Password"].Should().ContainSingle();
    }

    private sealed class TestEntity { }
}
