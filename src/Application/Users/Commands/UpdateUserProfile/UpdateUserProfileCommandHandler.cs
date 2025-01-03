using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUserProfile;

internal sealed class UpdateUserProfileCommandHandler(
    IUsersRepository usersRepository,
    IUserContext userContext)
    : IRequestHandler<UpdateUserProfileCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(
            userContext.UserId,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        user.UpdateProfile(command.FirstName, command.LastName);
        usersRepository.Update(user);

        return Result.Success;
    }
}