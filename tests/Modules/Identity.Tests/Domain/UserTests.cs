using ArchitecturePlayground.Modules.Identity.Core.Domain;
using ArchitecturePlayground.Modules.Identity.Core.Domain.Events;
using ArchitecturePlayground.Modules.Identity.Core.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ArchitecturePlayground.Modules.Identity.Tests.Domain;

public sealed class UserTests
{
    private readonly Email _validEmail;
    private readonly Password _validPassword;
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";

    public UserTests()
    {
        _validEmail = Email.Create("test@example.com").Value;
        _validPassword = Password.FromHash("$2a$11$validhash1234567890123456789012345").Value;
    }

    [Fact]
    public void Create_WithValidData_ReturnsUser()
    {
        // Act
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be(_validEmail);
        user.Password.Should().Be(_validPassword);
        user.FirstName.Should().Be(ValidFirstName);
        user.LastName.Should().Be(ValidLastName);
    }

    [Fact]
    public void Create_WithValidData_SetsDefaultValues()
    {
        // Act
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);

        // Assert
        user.Roles.Should().BeEmpty();
        user.EmailConfirmed.Should().BeFalse();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.LastLoginAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithValidData_EmitsUserCreatedEvent()
    {
        // Act
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First();
        domainEvent.Should().BeOfType<UserCreatedEvent>();

        var userCreatedEvent = (UserCreatedEvent)domainEvent;
        userCreatedEvent.UserId.Should().Be(user.Id);
        userCreatedEvent.Email.Should().Be(_validEmail.Value);
    }

    [Fact]
    public void ChangePassword_WithNewPassword_UpdatesPassword()
    {
        // Arrange
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);
        user.ClearDomainEvents();
        var newPassword = Password.FromHash("$2a$11$newhash12345678901234567890123456").Value;

        // Act
        user.ChangePassword(newPassword);

        // Assert
        user.Password.Should().Be(newPassword);
    }

    [Fact]
    public void ChangePassword_WithNewPassword_EmitsUserPasswordChangedEvent()
    {
        // Arrange
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);
        user.ClearDomainEvents();
        var newPassword = Password.FromHash("$2a$11$newhash12345678901234567890123456").Value;

        // Act
        user.ChangePassword(newPassword);

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First();
        domainEvent.Should().BeOfType<UserPasswordChangedEvent>();

        var passwordChangedEvent = (UserPasswordChangedEvent)domainEvent;
        passwordChangedEvent.UserId.Should().Be(user.Id);
        passwordChangedEvent.ChangedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ConfirmEmail_SetsEmailConfirmedToTrue()
    {
        // Arrange
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);

        // Act
        user.ConfirmEmail();

        // Assert
        user.EmailConfirmed.Should().BeTrue();
    }

    [Fact]
    public void RecordLogin_SetsLastLoginAt()
    {
        // Arrange
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);

        // Act
        user.RecordLogin();

        // Assert
        user.LastLoginAt.Should().NotBeNull();
        user.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddRole_AddsRoleToCollection()
    {
        // Arrange
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);

        // Act
        user.AddRole("Admin");

        // Assert
        user.Roles.Should().Contain("Admin");
    }

    [Fact]
    public void AddRole_WithDuplicateRole_DoesNotAddDuplicate()
    {
        // Arrange
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);
        user.AddRole("Admin");

        // Act
        user.AddRole("Admin");

        // Assert
        user.Roles.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveRole_RemovesRoleFromCollection()
    {
        // Arrange
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);
        user.AddRole("Admin");
        user.AddRole("Customer");

        // Act
        user.RemoveRole("Admin");

        // Assert
        user.Roles.Should().NotContain("Admin");
        user.Roles.Should().Contain("Customer");
    }

    [Fact]
    public void RemoveRole_WithNonExistingRole_DoesNothing()
    {
        // Arrange
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);
        user.AddRole("Customer");

        // Act
        user.RemoveRole("Admin");

        // Assert
        user.Roles.Should().HaveCount(1);
        user.Roles.Should().Contain("Customer");
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        // Arrange
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);
        user.DomainEvents.Should().NotBeEmpty();

        // Act
        user.ClearDomainEvents();

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Equals_WithSameId_ReturnsTrue()
    {
        // Arrange
        var user = User.Create(_validEmail, _validPassword, ValidFirstName, ValidLastName);

        // Act & Assert
        user.Equals(user).Should().BeTrue();
    }
}
