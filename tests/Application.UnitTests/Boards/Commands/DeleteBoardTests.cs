using Application.Boards.Commands.DeleteBoard;
using Domain.Boards;

namespace Application.UnitTests.Boards.Commands;

public class DeleteBoardTests
{
    private static readonly DeleteBoardCommand Command = new(Guid.NewGuid());

    private readonly DeleteBoardCommandHandler _handler;
    private readonly IBoardRepository _boardRepository;

    public DeleteBoardTests()
    {
        _boardRepository = Substitute.For<IBoardRepository>();
        _handler = new DeleteBoardCommandHandler(_boardRepository);
    }

    [Fact]
    public async Task Handle_WhenBoardDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _boardRepository.GetByIdAsync(
            Guid.NewGuid(),
            TestContext.Current.CancellationToken)
            .Returns(null as Board);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenBoardExists_ShouldDeleteBoard()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");
        _boardRepository.GetByIdAsync(
            Command.BoardId,
            TestContext.Current.CancellationToken)
            .Returns(board);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        _boardRepository.Received().Remove(board);
    }
}