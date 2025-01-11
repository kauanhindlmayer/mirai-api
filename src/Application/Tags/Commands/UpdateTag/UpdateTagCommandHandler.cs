using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.UpdateTag;

internal sealed class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, ErrorOr<Guid>>
{
    private readonly IProjectsRepository _projectsRepository;

    public UpdateTagCommandHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        UpdateTagCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithTagsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var tag = project.Tags.FirstOrDefault(t => t.Id == command.TagId);
        if (tag is null)
        {
            return TagErrors.NotFound;
        }

        tag.UpdateName(command.Name);
        _projectsRepository.Update(project);

        return tag.Id;
    }
}