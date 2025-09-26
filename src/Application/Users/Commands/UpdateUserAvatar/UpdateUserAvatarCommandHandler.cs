using Application.Abstractions.Authentication;
using Application.Abstractions.Storage;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUserAvatar;

internal sealed class UpdateUserAvatarCommandHandler
    : IRequestHandler<UpdateUserAvatarCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IBlobService _blobService;

    public UpdateUserAvatarCommandHandler(
        IUserRepository userRepository,
        IUserContext userContext,
        IBlobService blobService)
    {
        _userRepository = userRepository;
        _userContext = userContext;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateUserAvatarCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
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
        _userRepository.Update(user);

        return Result.Success;
    }
}