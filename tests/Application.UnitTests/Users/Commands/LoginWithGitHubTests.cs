using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Abstractions.Authentication;
using Application.Users.Commands.LoginWithGitHub;
using Domain.Shared;
using Domain.Users;

namespace Application.UnitTests.Users.Commands;

public class LoginWithGitHubTests
{
    private static readonly LoginWithGitHubCommand Command = new(
        "auth-code",
        "https://localhost:5173/auth/github/callback");

    private readonly LoginWithGitHubCommandHandler _handler;
    private readonly IUserRepository _mockUserRepository;
    private readonly IJwtService _mockJwtService;
    private readonly IUnitOfWork _mockUnitOfWork;

    public LoginWithGitHubTests()
    {
        _mockUserRepository = Substitute.For<IUserRepository>();
        _mockJwtService = Substitute.For<IJwtService>();
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new LoginWithGitHubCommandHandler(
            _mockUserRepository,
            _mockJwtService,
            _mockUnitOfWork);
    }

    [Fact]
    public async Task Handle_WhenUserExistsByIdentityId_ReturnsAccessTokenResponse()
    {
        // Arrange
        var identityId = Guid.NewGuid().ToString();
        var accessToken = CreateAccessToken(
            identityId,
            "john.doe@mirai.com",
            "John",
            "Doe");
        _mockJwtService.GetAccessTokenByAuthorizationCodeAsync(
            Command.Code,
            Command.RedirectUri,
            TestContext.Current.CancellationToken)
            .Returns(accessToken);
        var user = new User("John", "Doe", "john.doe@mirai.com");
        _mockUserRepository.GetByIdentityIdAsync(
            identityId,
            TestContext.Current.CancellationToken)
            .Returns(user);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.AccessToken.Should().Be(accessToken);
        await _mockUserRepository.DidNotReceive().GetByEmailAsync(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
        await _mockUserRepository.DidNotReceive().AddAsync(
            Arg.Any<User>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserExistsByEmail_LinksIdentityIdAndReturnsAccessTokenResponse()
    {
        // Arrange
        var identityId = Guid.NewGuid().ToString();
        var accessToken = CreateAccessToken(
            identityId,
            "john.doe@mirai.com",
            "John",
            "Doe");
        _mockJwtService.GetAccessTokenByAuthorizationCodeAsync(
            Command.Code,
            Command.RedirectUri,
            TestContext.Current.CancellationToken)
            .Returns(accessToken);
        _mockUserRepository.GetByIdentityIdAsync(
            identityId,
            TestContext.Current.CancellationToken)
            .Returns(null as User);
        var user = new User("John", "Doe", "john.doe@mirai.com");
        _mockUserRepository.GetByEmailAsync(
            "john.doe@mirai.com",
            TestContext.Current.CancellationToken)
            .Returns(user);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.AccessToken.Should().Be(accessToken);
        user.IdentityId.Should().Be(identityId);
        await _mockUserRepository.DidNotReceive().AddAsync(
            Arg.Any<User>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_CreatesUserAndReturnsAccessTokenResponse()
    {
        // Arrange
        var identityId = Guid.NewGuid().ToString();
        var accessToken = CreateAccessToken(
            identityId,
            "jane.doe@mirai.com",
            "Jane",
            "Doe");
        _mockJwtService.GetAccessTokenByAuthorizationCodeAsync(
            Command.Code,
            Command.RedirectUri,
            TestContext.Current.CancellationToken)
            .Returns(accessToken);
        _mockUserRepository.GetByIdentityIdAsync(
            identityId,
            TestContext.Current.CancellationToken)
            .Returns(null as User);
        _mockUserRepository.GetByEmailAsync(
            "jane.doe@mirai.com",
            TestContext.Current.CancellationToken)
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.AccessToken.Should().Be(accessToken);
        await _mockUserRepository.Received(1).AddAsync(
            Arg.Is<User>(u =>
                u.Email == "jane.doe@mirai.com" &&
                u.IdentityId == identityId),
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Handle_WhenAuthorizationCodeExchangeFails_ReturnsError()
    {
        // Arrange
        _mockJwtService.GetAccessTokenByAuthorizationCodeAsync(
            Command.Code,
            Command.RedirectUri,
            TestContext.Current.CancellationToken)
            .Returns(UserErrors.InvalidCredentials);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(UserErrors.InvalidCredentials);
    }

    private static string CreateAccessToken(
        string identityId,
        string email,
        string firstName,
        string lastName)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identityId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.GivenName, firstName),
            new(JwtRegisteredClaimNames.FamilyName, lastName),
        };

        var token = new JwtSecurityToken(claims: claims);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
