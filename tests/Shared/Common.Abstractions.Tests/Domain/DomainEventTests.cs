using ArchitecturePlayground.Common.Abstractions.Domain;
using FluentAssertions;

namespace ArchitecturePlayground.Common.Abstractions.Tests.Domain;

public sealed class DomainEventTests
{
    [Fact]
    public void DomainEvent_ShouldHaveUniqueId()
    {
        // Act
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        // Assert
        event1.Id.Should().NotBe(Guid.Empty);
        event2.Id.Should().NotBe(Guid.Empty);
        event1.Id.Should().NotBe(event2.Id);
    }

    [Fact]
    public void DomainEvent_ShouldHaveOccurredOnTimestamp()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var domainEvent = new TestDomainEvent();

        // Assert
        var after = DateTime.UtcNow;
        domainEvent.OccurredOn.Should().BeOnOrAfter(before);
        domainEvent.OccurredOn.Should().BeOnOrBefore(after);
    }

    private sealed class TestDomainEvent : DomainEvent
    {
    }
}
