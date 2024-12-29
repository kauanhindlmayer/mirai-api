using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUserProfilePicture;

internal sealed class UpdateUserProfilePictureCommandHandler(
    IUsersRepository usersRepository,
    IUserContext userContext,
    IBlobService blobService)
    : IRequestHandler<UpdateUserProfilePictureCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        UpdateUserProfilePictureCommand command,
        CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(
            userContext.UserId,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        var profilePictureUrl = await blobService.UploadAsync(
            command.File.OpenReadStream(),
            command.File.ContentType,
            cancellationToken: cancellationToken);

        user.SetImageUrl(profilePictureUrl);
        usersRepository.Update(user);

        return Result.Success;
    }
}