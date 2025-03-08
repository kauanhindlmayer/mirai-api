using Application.Boards.Commands.CreateColumn;
using Application.Common.Interfaces.Persistence;
using Domain.Boards;

namespace Application.UnitTests.Boards.Commands;

public class CreateColumnTests
{
    private static readonly CreateColumnCommand Command = new(
        Guid.NewGuid(),
        "Column",
        5,
        "Definition of done",
        0);

    private readonly CreateColumnCommandHandler _handler;
    private readonly IBoardsRepository _boardsRepository;

    public CreateColumnTests()
    {
        _boardsRepository = Substitute.For<IBoardsRepository>();
        _handler = new CreateColumnCommandHandler(_boardsRepository);
    }

    [Fact]
    public async Task Handle_WhenBoardDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(null as Board);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenPositionIsInvalid_ShouldReturnError()
    {
        // Arrange
        var board = new Board(Command.BoardId, "Board");
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        // Act
        var result = await _handler.Handle(
            Command with { Position = -1 },
            CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.InvalidPosition);
    }

    [Fact]
    public async Task Handle_WhenColumnAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var board = new Board(Command.BoardId, "Board");
        board.AddColumn(new BoardColumn(board.Id, Command.Name));
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenColumnDoesNotExist_ShouldAddColumn()
    {
        // Arrange
        var board = new Board(Command.BoardId, "Board");
        _boardsRepository.GetByIdWithColumnsAsync(Command.BoardId, Arg.Any<CancellationToken>())
            .Returns(board);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        board.Columns.Should().HaveCount(5);
        board.Columns.Should().ContainSingle(c => c.Name == Command.Name);
    }
}