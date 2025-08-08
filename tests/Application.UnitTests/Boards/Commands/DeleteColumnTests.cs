using Application.Boards.Commands.DeleteColumn;
using Domain.Boards;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.Boards.Commands;

public class DeleteColumnTests
{
    private static readonly DeleteColumnCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid());

    private readonly DeleteColumnCommandHandler _handler;
    private readonly IBoardsRepository _boardsRepository;

    public DeleteColumnTests()
    {
        _boardsRepository = Substitute.For<IBoardsRepository>();
        _handler = new DeleteColumnCommandHandler(_boardsRepository);
    }

    [Fact]
    public async Task Handle_WhenBoardDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, TestContext.Current.CancellationToken)
            .Returns(null as Board);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenColumnDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, TestContext.Current.CancellationToken)
            .Returns(board);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public async Task Handle_WhenColumnHasCards_ShouldReturnError()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var card = new BoardCard(column.Id, workItem.Id, 0);
        column.AddCard(card);
        board.AddColumn(column);
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, TestContext.Current.CancellationToken)
            .Returns(board);

        // Act
        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnHasCards(column));
    }

    [Fact]
    public async Task Handle_WhenColumnExists_ShouldRemoveColumn()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        board.AddColumn(column);
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, TestContext.Current.CancellationToken)
            .Returns(board);

        // Act
        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        board.Columns.Should().NotContain(column);
        _boardsRepository.Received().Update(board);
    }
}