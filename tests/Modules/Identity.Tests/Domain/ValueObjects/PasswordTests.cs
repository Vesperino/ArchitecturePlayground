using ArchitecturePlayground.Modules.Identity.Core.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ArchitecturePlayground.Modules.Identity.Tests.Domain.ValueObjects;

public sealed class PasswordTests
{
    [Fact]
    public void FromHash_WithValidHash_ReturnsSuccess()
    {
        // Arrange
        var hash = "$2a$11$validhashvaluethatislong123456789012345678901234567890";

        // Act
        var result = Password.FromHash(hash);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Hash.Should().Be(hash);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FromHash_WithEmptyOrNull_ReturnsFailure(string? hash)
    {
        // Act
        var result = Password.FromHash(hash);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Password.Empty");
    }

    [Fact]
    public void Equals_WithSameHash_ReturnsTrue()
    {
        // Arrange
        var hash = "$2a$11$somehashvalue123456789012345678901234567890";
        var password1 = Password.FromHash(hash).Value;
        var password2 = Password.FromHash(hash).Value;

        // Act & Assert
        password1.Should().Be(password2);
    }

    [Fact]
    public void Equals_WithDifferentHash_ReturnsFalse()
    {
        // Arrange
        var password1 = Password.FromHash("$2a$11$hash1234567890123456789012345678901234567890").Value;
        var password2 = Password.FromHash("$2a$11$differenthash12345678901234567890123456789").Value;

        // Act & Assert
        password1.Should().NotBe(password2);
    }

    [Fact]
    public void GetHashCode_WithSameHash_ReturnsSameValue()
    {
        // Arrange
        var hash = "$2a$11$somehashvalue123456789012345678901234567890";
        var password1 = Password.FromHash(hash).Value;
        var password2 = Password.FromHash(hash).Value;

        // Act & Assert
        password1.GetHashCode().Should().Be(password2.GetHashCode());
    }
}
