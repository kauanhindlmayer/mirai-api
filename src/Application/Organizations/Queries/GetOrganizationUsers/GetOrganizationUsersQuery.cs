using Application.Abstractions;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Queries.GetOrganizationUsers;

public sealed record GetOrganizationUsersQuery(
    Guid OrganizationId,
    int Page = 1,
    int PageSize = 10,
    string? Sort = null,
    string? SearchTerm = null) : IRequest<ErrorOr<PaginatedList<OrganizationUserResponse>>>;