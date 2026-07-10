using Domain.Authorization;
using MediatR;

namespace Application.Abstractions.Authorization;

public interface IAuthorizationRequest<TResponse> : IRequest<TResponse>, IAuthorizationRequest;

public interface IAuthorizationRequest
{
    Permission RequiredPermission { get; }
    ResourceType ResourceType { get; }
    Guid ResourceId { get; }
}
