using MediatR;

namespace ArchitecturePlayground.Common.Abstractions.CQRS;

/// <summary>
/// Marker interface for commands that don't return a value.
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Marker interface for commands that return a value.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
