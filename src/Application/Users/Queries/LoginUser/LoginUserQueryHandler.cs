using Application.Abstractions.Authentication;
using Domain.Shared;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.LoginUser;

internal sealed class LoginUserQueryHandler
    : IRequestHandler<LoginUserQuery, ErrorOr<AccessTokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserQueryHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AccessTokenResponse>> Handle(
        LoginUserQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            query.Email,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.InvalidCredentials;
        }

        var result = await _jwtService.GetAccessTokenAsync(
            query.Email,
            query.Password,
            cancellationToken);

        if (result.IsError)
        {
            return result.Errors;
        }

        user.UpdateLastActive();
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AccessTokenResponse(result.Value);
    }
}