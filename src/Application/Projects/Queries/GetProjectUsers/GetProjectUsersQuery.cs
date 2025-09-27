using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.GetProjectUsers;

public sealed record GetProjectUsersQuery(
    Guid ProjectId,
    string? SearchTerm = null) : IRequest<ErrorOr<List<ProjectUserResponse>>>;