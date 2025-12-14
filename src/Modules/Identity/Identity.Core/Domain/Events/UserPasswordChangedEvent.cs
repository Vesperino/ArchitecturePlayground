using ArchitecturePlayground.Common.Abstractions.Domain;

namespace ArchitecturePlayground.Modules.Identity.Core.Domain.Events;

public sealed class UserPasswordChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public DateTime ChangedAt { get; }

    public UserPasswordChangedEvent(Guid userId, DateTime changedAt)
    {
        UserId = userId;
        ChangedAt = changedAt;
    }
}
