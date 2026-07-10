using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Application.TagImportJobs.Commands.CreateTagImportJob;

public sealed record CreateTagImportJobCommand(
    Guid ProjectId,
    IFormFile File) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectManageTags;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
