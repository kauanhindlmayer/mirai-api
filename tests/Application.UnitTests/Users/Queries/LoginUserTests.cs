using Application.Abstractions.Authentication;
using Application.Users.Queries.LoginUser;
using Domain.Users;

namespace Application.UnitTests.Users.Queries;

public class LoginUserTests
{
    private static readonly LoginUserQuery Query = new(
        "john.doe@email.com",
        "password");

    private readonly LoginUserQueryHandler _handler;
    private readonly IUserRepository _mockuserRepository;
    private readonly IJwtService _mockJwtService;

    public LoginUserTests()
    {
        _mockuserRepository = Substitute.For<IUserRepository>();
        _mockJwtService = Substitute.For<IJwtService>();
        _handler = new LoginUserQueryHandler(
            _mockuserRepository,
            _mockJwtService);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        _mockuserRepository.GetByEmailAsync(Query.Email, TestContext.Current.CancellationToken)
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(Query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndPasswordMatches_ReturnsValidAccessTokenResponse()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@email.com");
        _mockuserRepository.GetByEmailAsync(Query.Email, TestContext.Current.CancellationToken)
            .Returns(user);

        var accessTokenResponse = new AccessTokenResponse("access_token");
        _mockJwtService.GetAccessTokenAsync(
            Query.Email,
            Query.Password,
            TestContext.Current.CancellationToken)
            .Returns(accessTokenResponse.AccessToken);

        // Act
        var result = await _handler.Handle(Query, TestContext.Current.CancellationToken);

        // Assert
        result.Value.Should().BeEquivalentTo(accessTokenResponse);
        await _mockuserRepository.Received(1).GetByEmailAsync(
            Query.Email,
            TestContext.Current.CancellationToken);
        await _mockJwtService.Received(1).GetAccessTokenAsync(
            Query.Email,
            Query.Password,
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndPasswordDoesNotMatch_ReturnsAuthenticationFailedError()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@email.com");
        _mockuserRepository.GetByEmailAsync(Query.Email, TestContext.Current.CancellationToken)
            .Returns(user);

        _mockJwtService.GetAccessTokenAsync(
            Query.Email,
            Query.Password,
            TestContext.Current.CancellationToken)
            .Returns(UserErrors.InvalidCredentials);

        // Act
        var result = await _handler.Handle(Query, TestContext.Current.CancellationToken);

        // Assert
        result.FirstError.Should().BeEquivalentTo(UserErrors.InvalidCredentials);
    }
}