using Application.Abstractions;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.GetProjectUsers;

public sealed record GetProjectUsersQuery(
    Guid ProjectId,
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null) : IRequest<ErrorOr<PaginatedList<ProjectUserResponse>>>;