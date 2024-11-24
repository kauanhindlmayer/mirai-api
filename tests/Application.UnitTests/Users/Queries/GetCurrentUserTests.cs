using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.Users.Queries.GetCurrentUser;
using Domain.Users;

namespace Application.UnitTests.Users.Queries;

public class GetCurrentUserTests
{
    private static readonly GetCurrentUserQuery Query = new();

    private readonly GetCurrentUserQueryHandler _handler;
    private readonly IUserContext _mockUserContext;
    private readonly IUsersRepository _mockUsersRepository;

    public GetCurrentUserTests()
    {
        _mockUserContext = Substitute.For<IUserContext>();
        _mockUsersRepository = Substitute.For<IUsersRepository>();
        _handler = new GetCurrentUserQueryHandler(
            _mockUserContext,
            _mockUsersRepository);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        _mockUserContext.UserId.Returns(Guid.NewGuid());
        _mockUsersRepository.GetByIdAsync(_mockUserContext.UserId, Arg.Any<CancellationToken>())
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@email.com");
        _mockUserContext.UserId.Returns(user.Id);
        _mockUsersRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _handler.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(user);
    }
}