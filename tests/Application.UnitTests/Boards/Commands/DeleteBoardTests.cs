using Application.Boards.Commands.DeleteBoard;
using Application.Common.Interfaces.Persistence;
using Domain.Boards;

namespace Application.UnitTests.Boards.Commands;

public class DeleteBoardTests
{
    private static readonly DeleteBoardCommand Command = new(Guid.NewGuid());

    private readonly DeleteBoardCommandHandler _handler;
    private readonly IBoardsRepository _boardsRepository;

    public DeleteBoardTests()
    {
        _boardsRepository = Substitute.For<IBoardsRepository>();
        _handler = new DeleteBoardCommandHandler(_boardsRepository);
    }

    [Fact]
    public async Task Handle_WhenBoardDoesNotExist_ShouldReturnError()
    {
        _boardsRepository.GetByIdAsync(Guid.NewGuid(), Arg.Any<CancellationToken>())
            .Returns(null as Board);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenBoardExists_ShouldDeleteBoard()
    {
        var board = new Board(Guid.NewGuid(), "Board");
        _boardsRepository.GetByIdAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.IsError.Should().BeFalse();
        _boardsRepository.Received().Remove(board);
    }
}