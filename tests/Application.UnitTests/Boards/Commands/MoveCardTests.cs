using Application.Boards.Commands.MoveCard;
using Application.Common.Interfaces.Persistence;
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
    private readonly IBoardsRepository _boardsRepository;

    public MoveCardTests()
    {
        _boardsRepository = Substitute.For<IBoardsRepository>();
        _handler = new MoveCardCommandHandler(_boardsRepository);
    }

    [Fact]
    public async Task Handle_WhenBoardDoesNotExist_ShouldReturnError()
    {
        _boardsRepository.GetByIdWithCardsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(null as Board);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenColumnDoesNotExist_ShouldReturnError()
    {
        var board = new Board(Guid.NewGuid(), "Board");
        _boardsRepository.GetByIdWithCardsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public async Task Handle_WhenCardDoesNotExist_ShouldReturnError()
    {
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var targetColumn = new BoardColumn(board.Id, "TargetColumn");
        board.AddColumn(column);
        board.AddColumn(targetColumn);
        _boardsRepository.GetByIdWithCardsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        var result = await _handler.Handle(
            Command with { ColumnId = column.Id, TargetColumnId = targetColumn.Id },
            CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.CardNotFound);
    }

    [Fact]
    public async Task Handle_WhenTargetColumnDoesNotExist_ShouldReturnError()
    {
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var card = column.AddCard(workItem);
        board.AddColumn(column);
        _boardsRepository.GetByIdWithCardsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        var result = await _handler.Handle(
            Command with { ColumnId = column.Id, CardId = card.Value.Id },
            CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public async Task Handle_PositionIsInvalid_ShouldReturnError()
    {
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var card = column.AddCard(workItem);
        var targetColumn = new BoardColumn(board.Id, "TargetColumn");
        board.AddColumn(column);
        board.AddColumn(targetColumn);
        _boardsRepository.GetByIdWithCardsAsync(board.Id, Arg.Any<CancellationToken>())
            .Returns(board);

        var command = new MoveCardCommand(
            board.Id,
            column.Id,
            card.Value.Id,
            targetColumn.Id,
            -1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.InvalidPosition);
    }

    [Fact]
    public async Task Handle_CardAlreadyExistsInTargetColumn_ShouldReturnError()
    {
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var targetColumn = new BoardColumn(board.Id, "TargetColumn");
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var card = column.AddCard(workItem);
        targetColumn.AddCard(workItem);
        board.AddColumn(column);
        board.AddColumn(targetColumn);
        _boardsRepository.GetByIdWithCardsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        var result = await _handler.Handle(
            Command with { ColumnId = column.Id, TargetColumnId = targetColumn.Id, CardId = card.Value.Id },
            CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.CardAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldMoveCard()
    {
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var targetColumn = new BoardColumn(board.Id, "TargetColumn");
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var card = column.AddCard(workItem);
        board.AddColumn(column);
        board.AddColumn(targetColumn);
        _boardsRepository.GetByIdWithCardsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);
        var command = Command with
        {
            ColumnId = column.Id,
            TargetColumnId = targetColumn.Id,
            CardId = card.Value.Id,
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
        column.Cards.Should().BeEmpty();
        targetColumn.Cards.Should().ContainSingle(c => c.WorkItemId == workItem.Id);
        _boardsRepository.Received().Update(board);
    }
}