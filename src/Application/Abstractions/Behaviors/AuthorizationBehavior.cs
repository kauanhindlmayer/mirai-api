using Application.Abstractions.Authentication;
using Application.Abstractions.Authorization;
using ErrorOr;
using MediatR;

namespace Application.Abstractions.Behaviors;

internal sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizationRequest
    where TResponse : IErrorOr
{
    private readonly IPermissionService _permissionService;
    private readonly IUserContext _userContext;

    public AuthorizationBehavior(
        IPermissionService permissionService,
        IUserContext userContext)
    {
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var hasPermission = await _permissionService.HasPermissionAsync(
            _userContext.UserId,
            request.RequiredPermission,
            request.ResourceType,
            request.ResourceId,
            cancellationToken);

        if (!hasPermission)
        {
            var errors = new List<Error> { AuthorizationErrors.Forbidden };
            return (dynamic)errors;
        }

        return await next(cancellationToken);
    }
}
