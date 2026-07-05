using Application.Abstractions;
using Application.Abstractions.Storage;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetUserAvatar;

internal sealed class GetUserAvatarQueryHandler
    : IRequestHandler<GetUserAvatarQuery, ErrorOr<FileResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBlobService _blobService;

    public GetUserAvatarQueryHandler(
        IApplicationDbContext context,
        IBlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<ErrorOr<FileResponse>> Handle(
        GetUserAvatarQuery query,
        CancellationToken cancellationToken)
    {
        var imageFileId = await _context.Users
            .Where(u => u.Id == query.UserId)
            .Select(u => u.ImageFileId)
            .FirstOrDefaultAsync(cancellationToken);

        if (imageFileId is null)
        {
            return UserErrors.NotFound;
        }

        return await _blobService.DownloadAsync(imageFileId.Value, cancellationToken);
    }
}
