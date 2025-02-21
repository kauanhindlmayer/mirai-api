using Domain.Boards;
using Domain.UnitTests.WorkItems;

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
        result.FirstError.Should().BeEquivalentTo(BoardErrors.ColumnAlreadyExists);
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
        board.Columns.Should().HaveCount(5);
        board.Columns.Last().Should().BeEquivalentTo(column);
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
        result.FirstError.Should().BeEquivalentTo(BoardErrors.ColumnNotFound);
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
        result.FirstError.Should().BeEquivalentTo(BoardErrors.ColumnHasCards(column));
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
        board.Columns.Should().HaveCount(4);
    }

    [Fact]
    public void RemoveColumn_WhenColumnDoesNotHaveCards_ShouldReorderColumns()
    {
        // Arrange
        var board = BoardFactory.CreateBoard();
        var column1 = BoardFactory.CreateBoardColumn(name: "Column 1");
        var column2 = BoardFactory.CreateBoardColumn(name: "Column 2");
        var column3 = BoardFactory.CreateBoardColumn(name: "Column 3");
        board.AddColumn(column1);
        board.AddColumn(column2);
        board.AddColumn(column3);

        // Act
        board.RemoveColumn(column2.Id);

        // Assert
        board.Columns.Should().HaveCount(6);
        board.Columns[^2].Position.Should().Be(4);
        board.Columns[^1].Position.Should().Be(5);
    }

    [Fact]
    public void RemoveColumn_WhenColumnDoesNotHaveCards_ShouldReorderColumnsWithGaps()
    {
        // Arrange
        var board = BoardFactory.CreateBoard();
        var column1 = BoardFactory.CreateBoardColumn(name: "Column 1");
        var column2 = BoardFactory.CreateBoardColumn(name: "Column 2");
        var column3 = BoardFactory.CreateBoardColumn(name: "Column 3");
        board.AddColumn(column1);
        board.AddColumn(column2);
        board.AddColumn(column3);

        // Act
        board.RemoveColumn(column1.Id);

        // Assert
        board.Columns.Should().HaveCount(6);
        board.Columns[^2].Position.Should().Be(4);
        board.Columns[^1].Position.Should().Be(5);
    }
}