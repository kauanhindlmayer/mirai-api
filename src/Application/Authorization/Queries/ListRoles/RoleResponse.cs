using Domain.Authorization;

namespace Application.Authorization.Queries.ListRoles;

public sealed record RoleResponse(Guid Id, string Name, RoleScope Scope);
