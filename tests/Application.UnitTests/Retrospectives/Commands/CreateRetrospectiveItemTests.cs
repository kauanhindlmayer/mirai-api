using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.Retrospectives.Commands.CreateRetrospectiveItem;
using Domain.Retrospectives;

namespace Application.UnitTests.Retrospectives.Commands;

public class CreateRetrospectiveItemTests
{
    private static readonly CreateRetrospectiveItemCommand Command = new(
        "Test content",
        Guid.NewGuid(),
        Guid.NewGuid());

    private readonly CreateRetrospectiveItemCommandHandler _handler;
    private readonly IRetrospectivesRepository _retrospectivesRepository;
    private readonly IUserContext _userContext;

    public CreateRetrospectiveItemTests()
    {
        _retrospectivesRepository = Substitute.For<IRetrospectivesRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new CreateRetrospectiveItemCommandHandler(
            _retrospectivesRepository,
            _userContext);
    }

    [Fact]
    public async Task Handle_WhenRetrospectiveDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, TestContext.Current.CancellationToken)
            .Returns(null as Retrospective);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenColumnDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var retrospective = new Retrospective("Test Retrospective", 5, null, Guid.NewGuid());
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, TestContext.Current.CancellationToken)
            .Returns(retrospective);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ColumnNotFound);
    }

    [Fact]
    public async Task Handle_WhenItemAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var retrospective = new Retrospective("Test Retrospective", 5, null, Guid.NewGuid());
        var column = new RetrospectiveColumn("Test Column", retrospective.Id);
        var item = new RetrospectiveItem("Test content", column.Id, Guid.NewGuid());
        column.AddItem(item);
        retrospective.AddColumn(column);
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, TestContext.Current.CancellationToken)
            .Returns(retrospective);

        // Act
        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ItemAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateItem()
    {
        // Arrange
        var retrospective = new Retrospective("Test Retrospective", 5, null, Guid.NewGuid());
        var column = new RetrospectiveColumn("Test Column", retrospective.Id);
        retrospective.AddColumn(column);
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, TestContext.Current.CancellationToken)
            .Returns(retrospective);

        // Act
        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Content.Should().Be(Command.Content);
        column.Items.Should().ContainSingle(i => i.Content == Command.Content);
    }
}