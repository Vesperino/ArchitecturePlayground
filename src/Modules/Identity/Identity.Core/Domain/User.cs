using ArchitecturePlayground.Common.Abstractions.Domain;
using ArchitecturePlayground.Modules.Identity.Core.Domain.Events;
using ArchitecturePlayground.Modules.Identity.Core.Domain.ValueObjects;

namespace ArchitecturePlayground.Modules.Identity.Core.Domain;

public sealed class User : AggregateRoot<Guid>
{
    public Email Email { get; private set; }
    public Password Password { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public HashSet<string> Roles { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private User(
        Guid id,
        Email email,
        Password password,
        string firstName,
        string lastName) : base(id)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        Roles = [];
        EmailConfirmed = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(
        Email email,
        Password password,
        string firstName,
        string lastName)
    {
        var user = new User(Guid.NewGuid(), email, password, firstName, lastName);
        user.AddDomainEvent(new UserCreatedEvent(user.Id, email.Value));
        return user;
    }

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword;
        AddDomainEvent(new UserPasswordChangedEvent(Id, DateTime.UtcNow));
    }

    public void ConfirmEmail() => EmailConfirmed = true;

    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;

    public void AddRole(string role) => Roles.Add(role);

    public void RemoveRole(string role) => Roles.Remove(role);

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value
    private User() : base(default) { } // EF Core
#pragma warning restore CS8618
}
