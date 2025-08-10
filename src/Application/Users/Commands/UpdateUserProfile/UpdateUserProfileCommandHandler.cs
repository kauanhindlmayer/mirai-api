using Application.Abstractions.Authentication;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUserProfile;

internal sealed class UpdateUserProfileCommandHandler
    : IRequestHandler<UpdateUserProfileCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        IUserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
            _userContext.UserId,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        user.UpdateProfile(command.FirstName, command.LastName);
        _userRepository.Update(user);

        return Result.Success;
    }
}