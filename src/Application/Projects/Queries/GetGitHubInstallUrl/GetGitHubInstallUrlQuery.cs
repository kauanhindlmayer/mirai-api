using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.GetGitHubInstallUrl;

public sealed record GetGitHubInstallUrlQuery(
    Guid OrganizationId,
    Guid ProjectId) : IRequest<ErrorOr<string>>;
