using Domain.Authorization;
using ErrorOr;
using MediatR;

namespace Application.Authorization.Queries.ListRoles;

public sealed record ListRolesQuery(RoleScope? Scope) : IRequest<ErrorOr<IReadOnlyList<RoleResponse>>>;
