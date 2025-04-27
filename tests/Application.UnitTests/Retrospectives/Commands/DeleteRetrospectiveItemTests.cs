using Application.Common.Interfaces.Persistence;
using Application.Retrospectives.Commands.DeleteRetrospectiveItem;
using Domain.Retrospectives;

namespace Application.UnitTests.Retrospectives.Commands;

public class DeleteRetrospectiveItemTests
{
    private static readonly DeleteRetrospectiveItemCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid());

    private readonly DeleteRetrospectiveItemCommandHandler _handler;
    private readonly IRetrospectivesRepository _retrospectivesRepository;

    public DeleteRetrospectiveItemTests()
    {
        _retrospectivesRepository = Substitute.For<IRetrospectivesRepository>();
        _handler = new DeleteRetrospectiveItemCommandHandler(_retrospectivesRepository);
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
    public async Task Handle_WhenItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var retrospective = new Retrospective("Test Retrospective", 5, null, Guid.NewGuid());
        var column = new RetrospectiveColumn("Test Column", Command.RetrospectiveId);
        retrospective.AddColumn(column);
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, TestContext.Current.CancellationToken)
            .Returns(retrospective);

        // Act
        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ItemNotFound);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldDeleteItem()
    {
        // Arrange
        var retrospective = new Retrospective("Test Retrospective", 5, null, Guid.NewGuid());
        var column = new RetrospectiveColumn("Test Column", Command.RetrospectiveId);
        var item = new RetrospectiveItem("Test content", column.Id, Command.ItemId);
        column.AddItem(item);
        retrospective.AddColumn(column);
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, TestContext.Current.CancellationToken)
            .Returns(retrospective);

        // Act
        var result = await _handler.Handle(
            Command with { ColumnId = column.Id, ItemId = item.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        column.Items.Should().NotContain(i => i.Id == Command.ItemId);
        _retrospectivesRepository.Received().Update(retrospective);
    }
}