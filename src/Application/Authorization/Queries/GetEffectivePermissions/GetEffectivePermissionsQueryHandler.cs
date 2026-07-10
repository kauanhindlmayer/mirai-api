using Application.Abstractions.Authentication;
using Application.Abstractions.Authorization;
using ErrorOr;
using MediatR;

namespace Application.Authorization.Queries.GetEffectivePermissions;

internal sealed class GetEffectivePermissionsQueryHandler
    : IRequestHandler<GetEffectivePermissionsQuery, ErrorOr<IReadOnlyList<string>>>
{
    private readonly IPermissionService _permissionService;
    private readonly IUserContext _userContext;

    public GetEffectivePermissionsQueryHandler(
        IPermissionService permissionService,
        IUserContext userContext)
    {
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<ErrorOr<IReadOnlyList<string>>> Handle(
        GetEffectivePermissionsQuery query,
        CancellationToken cancellationToken)
    {
        var permissions = await _permissionService.GetEffectivePermissionsAsync(
            _userContext.UserId,
            query.ResourceType,
            query.ResourceId,
            cancellationToken);

        return permissions.Select(p => p.ToString()).ToList();
    }
}
