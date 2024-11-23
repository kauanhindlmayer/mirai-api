using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.Users.Common;
using Application.Users.Queries.LoginUser;
using Domain.Users;

namespace Application.UnitTests.Users.Queries;

public class LoginUserTests
{
    private static readonly LoginUserQuery Query = new(
        "john.doe@email.com",
        "password");

    private readonly LoginUserQueryHandler _handler;
    private readonly IUsersRepository _mockUsersRepository;
    private readonly IJwtService _mockJwtService;

    public LoginUserTests()
    {
        _mockUsersRepository = Substitute.For<IUsersRepository>();
        _mockJwtService = Substitute.For<IJwtService>();
        _handler = new LoginUserQueryHandler(
            _mockUsersRepository,
            _mockJwtService);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        _mockUsersRepository.GetByEmailAsync(Query.Email, Arg.Any<CancellationToken>())
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(Query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(UserErrors.AuthenticationFailed);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndPasswordMatches_ReturnsValidAccessTokenResponse()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@email.com");
        _mockUsersRepository.GetByEmailAsync(Query.Email, Arg.Any<CancellationToken>())
            .Returns(user);

        var accessTokenResponse = new AccessTokenResponse("access_token");
        _mockJwtService.GetAccessTokenAsync(
            Query.Email,
            Query.Password,
            Arg.Any<CancellationToken>())
            .Returns(accessTokenResponse.AccessToken);

        // Act
        var result = await _handler.Handle(Query, CancellationToken.None);

        // Assert
        result.Value.Should().BeEquivalentTo(accessTokenResponse);
        await _mockUsersRepository.Received(1).GetByEmailAsync(
            Query.Email,
            Arg.Any<CancellationToken>());
        await _mockJwtService.Received(1).GetAccessTokenAsync(
            Query.Email,
            Query.Password,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndPasswordDoesNotMatch_ReturnsAuthenticationFailedError()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@email.com");
        _mockUsersRepository.GetByEmailAsync(Query.Email, Arg.Any<CancellationToken>())
            .Returns(user);

        _mockJwtService.GetAccessTokenAsync(
            Query.Email,
            Query.Password,
            Arg.Any<CancellationToken>())
            .Returns(UserErrors.AuthenticationFailed);

        // Act
        var result = await _handler.Handle(Query, CancellationToken.None);

        // Assert
        result.Errors.First().Should().BeEquivalentTo(UserErrors.AuthenticationFailed);
    }
}