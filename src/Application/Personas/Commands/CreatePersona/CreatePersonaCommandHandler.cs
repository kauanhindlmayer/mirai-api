using Application.Abstractions.Storage;
using Domain.Personas;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Personas.Commands.CreatePersona;

internal sealed class CreatePersonaCommandHandler : IRequestHandler<CreatePersonaCommand, ErrorOr<Guid>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IBlobService _blobService;

    public CreatePersonaCommandHandler(
        IProjectsRepository projectsRepository,
        IBlobService blobService)
    {
        _projectsRepository = projectsRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreatePersonaCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithPersonasAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var persona = new Persona(
            project.Id,
            command.Name,
            command.Category,
            command.Description);

        var result = project.AddPersona(persona);
        if (result.IsError)
        {
            return result.Errors;
        }

        if (command.File is not null)
        {
            var uploadResponse = await _blobService.UploadAsync(
                command.File.OpenReadStream(),
                command.File.ContentType,
                cancellationToken);

            persona.SetImage(
                uploadResponse.FileUrl,
                uploadResponse.FileId);
        }

        _projectsRepository.Update(project);

        return persona.Id;
    }
}