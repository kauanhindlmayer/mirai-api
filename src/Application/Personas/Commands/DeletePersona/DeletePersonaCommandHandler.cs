using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Personas.Commands.DeletePersona;

internal sealed class DeletePersonaCommandHandler
    : IRequestHandler<DeletePersonaCommand, ErrorOr<Success>>
{
    private readonly IProjectsRepository _projectsRepository;

    public DeletePersonaCommandHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeletePersonaCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithPersonasAsync(
            command.ProjectId,
            cancellationToken);
        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var result = project.RemovePersona(command.PersonaId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return Result.Success;
    }
}