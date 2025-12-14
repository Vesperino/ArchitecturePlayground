namespace ArchitecturePlayground.Common.Abstractions.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public sealed class NotFoundException : DomainException
{
    public string ResourceType { get; }
    public object? ResourceId { get; }

    public NotFoundException(string resourceType, object? resourceId)
        : base($"{resourceType} with id '{resourceId}' was not found.")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    public static NotFoundException For<T>(object? id)
    {
        return new NotFoundException(typeof(T).Name, id);
    }
}
