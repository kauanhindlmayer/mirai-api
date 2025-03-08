using Application.Boards.Commands.DeleteColumn;
using Application.Common.Interfaces.Persistence;
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
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(null as Board);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenColumnDoesNotExist_ShouldReturnError()
    {
        var board = new Board(Guid.NewGuid(), "Board");
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public async Task Handle_WhenColumnHasCards_ShouldReturnError()
    {
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        column.AddCard(workItem);
        board.AddColumn(column);
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnHasCards(column));
    }

    [Fact]
    public async Task Handle_WhenColumnExists_ShouldRemoveColumn()
    {
        var board = new Board(Guid.NewGuid(), "Board");
        var column = new BoardColumn(board.Id, "Column");
        board.AddColumn(column);
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            CancellationToken.None);

        result.IsError.Should().BeFalse();
        board.Columns.Should().NotContain(column);
        _boardsRepository.Received().Update(board);
    }
}