using ArchitecturePlayground.Common.Abstractions.Domain;
using FluentAssertions;

namespace ArchitecturePlayground.Common.Abstractions.Tests.Domain;

public sealed class ValueObjectTests
{
    [Fact]
    public void ValueObjects_WithSameProperties_ShouldBeEqual()
    {
        // Arrange
        var vo1 = new TestValueObject("test", 42);
        var vo2 = new TestValueObject("test", 42);

        // Assert
        vo1.Should().Be(vo2);
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void ValueObjects_WithDifferentProperties_ShouldNotBeEqual()
    {
        // Arrange
        var vo1 = new TestValueObject("test", 42);
        var vo2 = new TestValueObject("different", 42);

        // Assert
        vo1.Should().NotBe(vo2);
        (vo1 != vo2).Should().BeTrue();
    }

    [Fact]
    public void ValueObjects_WithSameProperties_ShouldHaveSameHashCode()
    {
        // Arrange
        var vo1 = new TestValueObject("test", 42);
        var vo2 = new TestValueObject("test", 42);

        // Assert
        vo1.GetHashCode().Should().Be(vo2.GetHashCode());
    }

    [Fact]
    public void ValueObject_ComparedToNull_ShouldNotBeEqual()
    {
        // Arrange
        var vo = new TestValueObject("test", 42);

        // Assert
        vo.Equals(null).Should().BeFalse();
    }

    private sealed class TestValueObject : ValueObject
    {
        public string Name { get; }
        public int Value { get; }

        public TestValueObject(string name, int value)
        {
            Name = name;
            Value = value;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Name;
            yield return Value;
        }
    }
}
