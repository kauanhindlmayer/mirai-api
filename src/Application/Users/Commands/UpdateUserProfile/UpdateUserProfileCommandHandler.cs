using Application.Abstractions.Authentication;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUserProfile;

internal sealed class UpdateUserProfileCommandHandler
    : IRequestHandler<UpdateUserProfileCommand, ErrorOr<Success>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUserContext _userContext;

    public UpdateUserProfileCommandHandler(
        IUsersRepository usersRepository,
        IUserContext userContext)
    {
        _usersRepository = usersRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(
            _userContext.UserId,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        user.UpdateProfile(command.FirstName, command.LastName);
        _usersRepository.Update(user);

        return Result.Success;
    }
}