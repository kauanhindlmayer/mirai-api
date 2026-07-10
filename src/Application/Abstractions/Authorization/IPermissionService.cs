using Domain.Authorization;

namespace Application.Abstractions.Authorization;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(
        Guid userId,
        Permission permission,
        ResourceType resourceType,
        Guid resourceId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Permission>> GetEffectivePermissionsAsync(
        Guid userId,
        ResourceType resourceType,
        Guid resourceId,
        CancellationToken cancellationToken = default);
}
