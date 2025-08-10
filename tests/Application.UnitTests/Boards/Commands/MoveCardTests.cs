using Application.Boards.Commands.MoveCard;
using Domain.Boards;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.Boards.Commands;

public class MoveCardTests
{
    private static readonly MoveCardCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid(),
        0);

    private readonly MoveCardCommandHandler _handler;
    private readonly IBoardRepository _boardRepository;

    public MoveCardTests()
    {
        _boardRepository = Substitute.For<IBoardRepository>();
        _handler = new MoveCardCommandHandler(_boardRepository);
    }

    [Fact]
    public async Task Handle_WhenBoardDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _boardRepository.GetByIdWithCardsAsync(Command.BoardId, TestContext.Current.CancellationToken)
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
        _boardRepository.GetByIdWithCardsAsync(Command.BoardId, TestContext.Current.CancellationToken)
            .Returns(board);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public async Task Handle_WhenCardDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var targetColumn = new BoardColumn(board.Id, "TargetColumn");
        board.AddColumn(column);
        board.AddColumn(targetColumn);
        _boardRepository.GetByIdWithCardsAsync(Command.BoardId, TestContext.Current.CancellationToken)
            .Returns(board);

        // Act
        var result = await _handler.Handle(
            Command with { ColumnId = column.Id, TargetColumnId = targetColumn.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.CardNotFound);
    }

    [Fact]
    public async Task Handle_WhenTargetColumnDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var card = new BoardCard(column.Id, workItem.Id, 0);
        column.AddCard(card);
        board.AddColumn(column);
        _boardRepository.GetByIdWithCardsAsync(Command.BoardId, TestContext.Current.CancellationToken)
            .Returns(board);

        // Act
        var result = await _handler.Handle(
            Command with { ColumnId = column.Id, CardId = card.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public async Task Handle_WhenTargetPositionIsInvalid_ShouldReturnError()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var targetColumn = new BoardColumn(board.Id, "TargetColumn");
        board.AddColumn(column);
        board.AddColumn(targetColumn);
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var card = new BoardCard(column.Id, workItem.Id, 0);
        column.AddCard(card);
        _boardRepository.GetByIdWithCardsAsync(board.Id, TestContext.Current.CancellationToken)
            .Returns(board);
        var command = new MoveCardCommand(
            board.Id,
            column.Id,
            card.Id,
            targetColumn.Id,
            TargetPosition: -1);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.InvalidPosition);
    }

    [Fact]
    public async Task Handle_CardAlreadyExistsInTargetColumn_ShouldReturnError()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var targetColumn = new BoardColumn(board.Id, "TargetColumn");
        board.AddColumn(column);
        board.AddColumn(targetColumn);
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var card = new BoardCard(column.Id, workItem.Id, 0);
        column.AddCard(card);
        targetColumn.AddCard(card);
        _boardRepository.GetByIdWithCardsAsync(board.Id, TestContext.Current.CancellationToken)
            .Returns(board);
        var command = new MoveCardCommand(
            board.Id,
            column.Id,
            card.Id,
            targetColumn.Id,
            0);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.CardAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldMoveCard()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var targetColumn = new BoardColumn(board.Id, "TargetColumn");
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var card = new BoardCard(column.Id, workItem.Id, 0);
        column.AddCard(card);
        board.AddColumn(column);
        board.AddColumn(targetColumn);
        _boardRepository.GetByIdWithCardsAsync(board.Id, TestContext.Current.CancellationToken)
            .Returns(board);
        var command = new MoveCardCommand(
            board.Id,
            column.Id,
            card.Id,
            targetColumn.Id,
            0);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
        column.Cards.Should().BeEmpty();
        targetColumn.Cards.Should().ContainSingle(c => c.WorkItemId == workItem.Id);
        _boardRepository.Received().Update(board);
    }
}