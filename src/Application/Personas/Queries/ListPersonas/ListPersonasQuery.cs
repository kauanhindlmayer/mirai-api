using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Personas.Queries.ListPersonas;

public sealed record ListPersonasQuery(Guid ProjectId)
    : IAuthorizationRequest<ErrorOr<IReadOnlyList<PersonaBriefResponse>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
