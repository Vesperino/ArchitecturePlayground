namespace ArchitecturePlayground.Common.Abstractions.Domain;

/// <summary>
/// Base class for domain events with auto-generated Id and timestamp.
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }

    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}
