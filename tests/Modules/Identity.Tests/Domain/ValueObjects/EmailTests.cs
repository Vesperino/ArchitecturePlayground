using ArchitecturePlayground.Modules.Identity.Core.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ArchitecturePlayground.Modules.Identity.Tests.Domain.ValueObjects;

public sealed class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ReturnsSuccess()
    {
        // Arrange
        var emailValue = "test@example.com";

        // Act
        var result = Email.Create(emailValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(emailValue);
    }

    [Fact]
    public void Create_WithValidEmail_NormalizesToLowercase()
    {
        // Arrange
        var emailValue = "Test@EXAMPLE.COM";

        // Act
        var result = Email.Create(emailValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void Create_WithValidEmail_TrimsWhitespace()
    {
        // Arrange
        var emailValue = "  test@example.com  ";

        // Act
        var result = Email.Create(emailValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("test@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrNull_ReturnsFailure(string? emailValue)
    {
        // Act
        var result = Email.Create(emailValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Email.Empty");
    }

    [Fact]
    public void Create_WithTooLongEmail_ReturnsFailure()
    {
        // Arrange - Email exceeding 254 characters (RFC 5321)
        var localPart = new string('a', 64);
        var domain = new string('b', 189) + ".com"; // Total > 254
        var emailValue = $"{localPart}@{domain}";

        // Act
        var result = Email.Create(emailValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Email.TooLong");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@example.com")]
    [InlineData("test@.com")]
    [InlineData("test@example")]
    [InlineData("test@@example.com")]
    public void Create_WithInvalidFormat_ReturnsFailure(string emailValue)
    {
        // Act
        var result = Email.Create(emailValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Email.InvalidFormat");
    }

    [Theory]
    [InlineData("user..name@example.com")]  // Double dots
    [InlineData(".user@example.com")]        // Starts with dot
    [InlineData("user.@example.com")]        // Ends with dot before @
    public void Create_WithInvalidRfcFormat_ReturnsFailure(string emailValue)
    {
        // Act
        var result = Email.Create(emailValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Email.InvalidFormat");
    }

    [Theory]
    [InlineData("user@-example.com")]        // Domain starts with hyphen
    [InlineData("user@example-.com")]        // Domain segment ends with hyphen
    public void Create_WithInvalidDomainFormat_ReturnsFailure(string emailValue)
    {
        // Act
        var result = Email.Create(emailValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Email.InvalidFormat");
    }

    [Theory]
    [InlineData("user+tag@example.com")]     // Plus addressing (Gmail)
    [InlineData("user.name@example.com")]    // Dots in local part
    [InlineData("user@sub.domain.com")]      // Subdomains
    [InlineData("user@example.co.uk")]       // Multiple TLD parts
    public void Create_WithValidRfcFormats_ReturnsSuccess(string emailValue)
    {
        // Act
        var result = Email.Create(emailValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithSameEmail_ReturnsTrue()
    {
        // Arrange
        var email1 = Email.Create("test@example.com").Value;
        var email2 = Email.Create("TEST@EXAMPLE.COM").Value; // Different case

        // Act & Assert
        email1.Should().Be(email2);
    }

    [Fact]
    public void Equals_WithDifferentEmail_ReturnsFalse()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com").Value;
        var email2 = Email.Create("test2@example.com").Value;

        // Act & Assert
        email1.Should().NotBe(email2);
    }

    [Fact]
    public void ToString_ReturnsEmailValue()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;

        // Act & Assert
        email.ToString().Should().Be("test@example.com");
    }
}
