using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Personas;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Personas.Commands.UpdatePersona;

internal sealed class UpdatePersonaCommandHandler : IRequestHandler<UpdatePersonaCommand, ErrorOr<Success>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IBlobService _blobService;

    public UpdatePersonaCommandHandler(
        IProjectsRepository projectsRepository,
        IBlobService blobService)
    {
        _projectsRepository = projectsRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdatePersonaCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithPersonasAsync(
            command.ProjectId,
            cancellationToken);
        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var persona = project.Personas.FirstOrDefault(p => p.Id == command.PersonaId);
        if (persona is null)
        {
            return PersonaErrors.NotFound;
        }

        if (command.File is not null)
        {
            if (persona.ImageFileId.HasValue)
            {
                await _blobService.DeleteAsync(
                    persona.ImageFileId.Value,
                    cancellationToken);
            }

            var uploadResponse = await _blobService.UploadAsync(
                command.File.OpenReadStream(),
                command.File.ContentType,
                cancellationToken);

            persona.SetImage(
                uploadResponse.FileUrl,
                uploadResponse.FileId);
        }

        persona.Update(command.Name, command.Description);
        _projectsRepository.Update(project);

        return Result.Success;
    }
}