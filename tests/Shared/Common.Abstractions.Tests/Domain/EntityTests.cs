using ArchitecturePlayground.Common.Abstractions.Domain;
using FluentAssertions;

namespace ArchitecturePlayground.Common.Abstractions.Tests.Domain;

public sealed class EntityTests
{
    [Fact]
    public void Entity_ShouldStoreId()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var entity = new TestEntity(id);

        // Assert
        entity.Id.Should().Be(id);
    }

    [Fact]
    public void Entities_WithSameId_ShouldBeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Assert
        entity1.Should().Be(entity2);
        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void Entities_WithDifferentIds_ShouldNotBeEqual()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Assert
        entity1.Should().NotBe(entity2);
        (entity1 != entity2).Should().BeTrue();
    }

    [Fact]
    public void Entity_ComparedToNull_ShouldNotBeEqual()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());

        // Assert
        entity.Equals(null).Should().BeFalse();
        (entity == null).Should().BeFalse();
    }

    [Fact]
    public void Entities_WithSameId_ShouldHaveSameHashCode()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Assert
        entity1.GetHashCode().Should().Be(entity2.GetHashCode());
    }

    [Fact]
    public void Entity_WithIntId_ShouldWork()
    {
        // Arrange
        var entity = new TestIntEntity(42);

        // Assert
        entity.Id.Should().Be(42);
    }

    private sealed class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) : base(id) { }
    }

    private sealed class TestIntEntity : Entity<int>
    {
        public TestIntEntity(int id) : base(id) { }
    }
}
