using System.IdentityModel.Tokens.Jwt;
using Application.Abstractions.Authentication;
using Application.Users.Queries.LoginUser;
using Domain.Shared;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.LoginWithGitHub;

internal sealed class LoginWithGitHubCommandHandler
    : IRequestHandler<LoginWithGitHubCommand, ErrorOr<AccessTokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginWithGitHubCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AccessTokenResponse>> Handle(
        LoginWithGitHubCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _jwtService.GetAccessTokenByAuthorizationCodeAsync(
            command.Code,
            command.RedirectUri,
            cancellationToken);

        if (result.IsError)
        {
            return result.Errors;
        }

        var accessToken = result.Value;
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

        string identityId = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        string email = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
        string firstName = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.GivenName).Value;
        string lastName = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.FamilyName).Value;

        var user = await _userRepository.GetByIdentityIdAsync(identityId, cancellationToken);

        if (user is not null)
        {
            user.UpdateLastActive();
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new AccessTokenResponse(accessToken);
        }

        user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is not null)
        {
            user.SetIdentityId(identityId);
            user.UpdateLastActive();
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new AccessTokenResponse(accessToken);
        }

        user = new User(firstName, lastName, email);
        user.SetIdentityId(identityId);
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AccessTokenResponse(accessToken);
    }
}
