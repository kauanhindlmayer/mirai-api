using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Personas.Commands.DeletePersona;

internal sealed class DeletePersonaCommandHandler
    : IRequestHandler<DeletePersonaCommand, ErrorOr<Success>>
{
    private readonly IProjectRepository _projectRepository;

    public DeletePersonaCommandHandler(
        IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeletePersonaCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithPersonasAsync(
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

        _projectRepository.Update(project);

        return Result.Success;
    }
}