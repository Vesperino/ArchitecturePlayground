using ArchitecturePlayground.Common.Abstractions.Domain;
using FluentAssertions;

namespace ArchitecturePlayground.Common.Abstractions.Tests.Domain;

public sealed class AggregateRootTests
{
    [Fact]
    public void AggregateRoot_ShouldStartWithNoDomainEvents()
    {
        // Act
        var aggregate = new TestAggregate(Guid.NewGuid());

        // Assert
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AddDomainEvent_ShouldAddEventToCollection()
    {
        // Arrange
        var aggregate = new TestAggregate(Guid.NewGuid());
        var domainEvent = new TestDomainEvent();

        // Act
        aggregate.TestAddDomainEvent(domainEvent);

        // Assert
        aggregate.DomainEvents.Should().ContainSingle()
            .Which.Should().Be(domainEvent);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.TestAddDomainEvent(new TestDomainEvent());
        aggregate.TestAddDomainEvent(new TestDomainEvent());

        // Act
        aggregate.ClearDomainEvents();

        // Assert
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AggregateRoot_ShouldInheritEntityEquality()
    {
        // Arrange
        var id = Guid.NewGuid();
        var aggregate1 = new TestAggregate(id);
        var aggregate2 = new TestAggregate(id);

        // Assert
        aggregate1.Should().Be(aggregate2);
    }

    private sealed class TestAggregate : AggregateRoot<Guid>
    {
        public TestAggregate(Guid id) : base(id) { }

        public void TestAddDomainEvent(IDomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
        }
    }

    private sealed class TestDomainEvent : DomainEvent
    {
    }
}
