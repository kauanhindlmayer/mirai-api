using Application.Abstractions;
using Application.Abstractions.Storage;
using Domain.Personas;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Personas.Queries.GetPersonaAvatar;

internal sealed class GetPersonaAvatarQueryHandler
    : IRequestHandler<GetPersonaAvatarQuery, ErrorOr<FileResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBlobService _blobService;

    public GetPersonaAvatarQueryHandler(
        IApplicationDbContext context,
        IBlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<ErrorOr<FileResponse>> Handle(
        GetPersonaAvatarQuery query,
        CancellationToken cancellationToken)
    {
        var imageFileId = await _context.Personas
            .Where(p => p.Id == query.PersonaId)
            .Select(p => p.ImageFileId)
            .FirstOrDefaultAsync(cancellationToken);

        if (imageFileId is null)
        {
            return PersonaErrors.NotFound;
        }

        return await _blobService.DownloadAsync(imageFileId.Value, cancellationToken);
    }
}
