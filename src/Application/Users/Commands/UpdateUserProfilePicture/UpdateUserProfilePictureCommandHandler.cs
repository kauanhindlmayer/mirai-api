using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUserProfilePicture;

internal sealed class UpdateUserProfilePictureCommandHandler
    : IRequestHandler<UpdateUserProfilePictureCommand, ErrorOr<Success>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUserContext _userContext;
    private readonly IBlobService _blobService;

    public UpdateUserProfilePictureCommandHandler(
        IUsersRepository usersRepository,
        IUserContext userContext,
        IBlobService blobService)
    {
        _usersRepository = usersRepository;
        _userContext = userContext;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateUserProfilePictureCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(
            _userContext.UserId,
            cancellationToken);
        if (user is null)
        {
            return UserErrors.NotFound;
        }

        if (user.ImageFileId.HasValue)
        {
            await _blobService.DeleteAsync(
                user.ImageFileId.Value,
                cancellationToken);
        }

        var uploadResponse = await _blobService.UploadAsync(
            command.File.OpenReadStream(),
            command.File.ContentType,
            cancellationToken: cancellationToken);

        user.SetImage(
            uploadResponse.FileUrl,
            uploadResponse.FileId);
        _usersRepository.Update(user);

        return Result.Success;
    }
}