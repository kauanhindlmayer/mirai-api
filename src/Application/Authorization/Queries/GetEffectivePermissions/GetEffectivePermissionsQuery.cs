using Domain.Authorization;
using ErrorOr;
using MediatR;

namespace Application.Authorization.Queries.GetEffectivePermissions;

public sealed record GetEffectivePermissionsQuery(
    ResourceType ResourceType,
    Guid ResourceId) : IRequest<ErrorOr<IReadOnlyList<string>>>;
