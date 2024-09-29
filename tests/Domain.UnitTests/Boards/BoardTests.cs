using Domain.Boards;
using TestCommon.Boards;
using TestCommon.WorkItems;

namespace Domain.UnitTests.Boards;

public class BoardTests
{
    [Fact]
    public void AddColumn_WhenColumnWithSameNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.CreateBoard();
        var column = BoardFactory.CreateBoardColumn();
        board.AddColumn(column);

        // Act
        var result = board.AddColumn(column);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(BoardErrors.ColumnAlreadyExists);
    }

    [Fact]
    public void AddColumn_WhenColumnWithSameNameDoesNotExists_ShouldAddColumn()
    {
        // Arrange
        var board = BoardFactory.CreateBoard();
        var column = BoardFactory.CreateBoardColumn();

        // Act
        var result = board.AddColumn(column);

        // Assert
        result.IsError.Should().BeFalse();
        board.Columns.Should().HaveCount(1);
        board.Columns.First().Should().BeEquivalentTo(column);
    }

    [Fact]
    public void RemoveColumn_WhenColumnDoesNotExists_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.CreateBoard();
        var column = BoardFactory.CreateBoardColumn();

        // Act
        var result = board.RemoveColumn(column.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public void RemoveColumn_WhenColumnHasCards_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.CreateBoard();
        var column = BoardFactory.CreateBoardColumn();
        var workItem = WorkItemFactory.CreateWorkItem();
        column.AddCard(workItem);
        board.AddColumn(column);

        // Act
        var result = board.RemoveColumn(column.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(BoardErrors.ColumnHasCards(column));
    }

    [Fact]
    public void RemoveColumn_WhenColumnDoesNotHaveCards_ShouldRemoveColumn()
    {
        // Arrange
        var board = BoardFactory.CreateBoard();
        var column = BoardFactory.CreateBoardColumn();
        board.AddColumn(column);

        // Act
        var result = board.RemoveColumn(column.Id);

        // Assert
        result.IsError.Should().BeFalse();
        board.Columns.Should().BeEmpty();
    }

    [Fact]
    public void RemoveColumn_WhenColumnDoesNotHaveCards_ShouldReorderColumns()
    {
        // Arrange
        var board = BoardFactory.CreateBoard();
        var column1 = BoardFactory.CreateBoardColumn(name: "Column 1", position: 0);
        var column2 = BoardFactory.CreateBoardColumn(name: "Column 2", position: 1);
        var column3 = BoardFactory.CreateBoardColumn(name: "Column 3", position: 2);
        board.AddColumn(column1);
        board.AddColumn(column2);
        board.AddColumn(column3);

        // Act
        board.RemoveColumn(column2.Id);

        // Assert
        board.Columns.Should().HaveCount(2);
        board.Columns.First().Position.Should().Be(0);
        board.Columns.Last().Position.Should().Be(1);
    }

    [Fact]
    public void RemoveColumn_WhenColumnDoesNotHaveCards_ShouldReorderColumnsWithGaps()
    {
        // Arrange
        var board = BoardFactory.CreateBoard();
        var column1 = BoardFactory.CreateBoardColumn(name: "Column 1", position: 0);
        var column2 = BoardFactory.CreateBoardColumn(name: "Column 2", position: 1);
        var column3 = BoardFactory.CreateBoardColumn(name: "Column 3", position: 2);
        board.AddColumn(column1);
        board.AddColumn(column2);
        board.AddColumn(column3);

        // Act
        board.RemoveColumn(column1.Id);

        // Assert
        board.Columns.Should().HaveCount(2);
        board.Columns.First().Position.Should().Be(0);
        board.Columns.Last().Position.Should().Be(1);
    }
}