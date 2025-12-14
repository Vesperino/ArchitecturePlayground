using ArchitecturePlayground.Common.Abstractions.Domain;

namespace ArchitecturePlayground.Modules.Identity.Core.Domain.Events;

public sealed class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }

    public UserCreatedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}
