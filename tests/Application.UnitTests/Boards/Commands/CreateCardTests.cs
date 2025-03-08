using Application.Boards.Commands.CreateCard;
using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.Teams;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.Boards.Commands;

public class CreateCardTests
{
    private static readonly CreateCardCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        WorkItemType.UserStory,
        "Title");

    private readonly CreateCardCommandHandler _handler;
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly IBoardsRepository _boardsRepository;

    public CreateCardTests()
    {
        _workItemsRepository = Substitute.For<IWorkItemsRepository>();
        _boardsRepository = Substitute.For<IBoardsRepository>();
        _handler = new CreateCardCommandHandler(
            _workItemsRepository,
            _boardsRepository);
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
        var team = new Team(Guid.NewGuid(), "Team", "Description");
        var board = new Board(team, "Board");
        _boardsRepository.GetByIdWithCardsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);
        _workItemsRepository.GetNextWorkItemCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(1);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public async Task Handle_ColumnExists_ShouldCreateCard()
    {
        var team = new Team(Guid.NewGuid(), "Team", "Description");
        var board = new Board(team, "Board");
        var column = new BoardColumn(board.Id, "Column");
        board.AddColumn(column);
        _boardsRepository.GetByIdWithCardsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);
        _workItemsRepository.GetNextWorkItemCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(1);

        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        _boardsRepository.Received().Update(board);
    }
}