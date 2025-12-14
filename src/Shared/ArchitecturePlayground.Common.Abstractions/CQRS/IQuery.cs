using MediatR;

namespace ArchitecturePlayground.Common.Abstractions.CQRS;

/// <summary>
/// Marker interface for queries.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
