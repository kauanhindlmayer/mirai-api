using Application.Abstractions.Authentication;
using Application.Abstractions.Authorization;
using Application.Abstractions.Behaviors;
using Domain.Authorization;
using MediatR;

namespace Application.UnitTests.Abstractions.Behaviors;

public class AuthorizationBehaviorTests
{
    private sealed record FakeAuthorizedCommand(Guid ResourceId) : IAuthorizationRequest<ErrorOr<Guid>>
    {
        public Permission RequiredPermission => Permission.ProjectManage;
        public ResourceType ResourceType => ResourceType.Project;
    }

    private readonly AuthorizationBehavior<FakeAuthorizedCommand, ErrorOr<Guid>> _behavior;
    private readonly IPermissionService _mockPermissionService;
    private readonly IUserContext _mockUserContext;
    private readonly RequestHandlerDelegate<ErrorOr<Guid>> _mockNextBehavior;

    public AuthorizationBehaviorTests()
    {
        _mockPermissionService = Substitute.For<IPermissionService>();
        _mockUserContext = Substitute.For<IUserContext>();
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<Guid>>>();
        _behavior = new AuthorizationBehavior<FakeAuthorizedCommand, ErrorOr<Guid>>(
            _mockPermissionService,
            _mockUserContext);
    }

    [Fact]
    public async Task Handle_WhenUserHasPermission_ShouldInvokeNextBehavior()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new FakeAuthorizedCommand(Guid.NewGuid());
        var expectedId = Guid.NewGuid();
        _mockUserContext.UserId.Returns(userId);
        _mockPermissionService.HasPermissionAsync(
            userId,
            command.RequiredPermission,
            command.ResourceType,
            command.ResourceId,
            TestContext.Current.CancellationToken)
            .Returns(true);
        _mockNextBehavior.Invoke(TestContext.Current.CancellationToken).Returns(expectedId);

        // Act
        var result = await _behavior.Handle(
            command,
            _mockNextBehavior,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(expectedId);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotHavePermission_ShouldReturnForbiddenError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new FakeAuthorizedCommand(Guid.NewGuid());
        _mockUserContext.UserId.Returns(userId);
        _mockPermissionService.HasPermissionAsync(
            userId,
            command.RequiredPermission,
            command.ResourceType,
            command.ResourceId,
            TestContext.Current.CancellationToken)
            .Returns(false);

        // Act
        var result = await _behavior.Handle(
            command,
            _mockNextBehavior,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AuthorizationErrors.Forbidden);
        await _mockNextBehavior.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }
}
