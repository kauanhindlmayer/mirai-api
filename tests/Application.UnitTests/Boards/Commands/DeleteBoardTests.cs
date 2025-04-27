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
        // Arrange
        _boardsRepository.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken)
            .Returns(null as Board);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenBoardExists_ShouldDeleteBoard()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");
        _boardsRepository.GetByIdAsync(Command.BoardId, TestContext.Current.CancellationToken)
            .Returns(board);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        _boardsRepository.Received().Remove(board);
    }
}