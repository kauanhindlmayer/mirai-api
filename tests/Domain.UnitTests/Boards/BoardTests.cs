using Domain.Boards;
using Domain.UnitTests.WorkItems;

namespace Domain.UnitTests.Boards;

public class BoardTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Act
        var board = BoardFactory.Create();

        // Assert
        board.Name.Should().Be(BoardFactory.Name);
        board.TeamId.Should().Be(BoardFactory.TeamId);
        board.Columns.Should().HaveCount(4);
    }

    [Fact]
    public void AddColumn_WhenColumnWithSameNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create();
        board.AddColumn(column);

        // Act
        var result = board.AddColumn(column);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.ColumnAlreadyExists);
    }

    [Fact]
    public void AddColumn_WhenColumnDoesNotExists_ShouldAddColumn()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create();

        // Act
        var result = board.AddColumn(column);

        // Assert
        result.IsError.Should().BeFalse();
        board.Columns.Should().HaveCount(5);
        board.Columns.Last().Should().BeEquivalentTo(column);
    }

    [Fact]
    public void AddColumnAtPosition_WhenPositionIsInvalid_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create();

        // Act
        var result = board.AddColumnAtPosition(column, -1);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.InvalidPosition);
    }

    [Fact]
    public void AddColumnAtPosition_WhenColumnAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create();
        board.AddColumn(column);

        // Act
        var result = board.AddColumnAtPosition(column, 0);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.ColumnAlreadyExists);
    }

    [Fact]
    public void AddColumnAtPosition_WhenPositionIsValid_ShouldAddColumn()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create(board.Id, name: "New Column");

        // Act
        var result = board.AddColumnAtPosition(column, 0);

        // Assert
        result.IsError.Should().BeFalse();
        board.Columns.Should().HaveCount(5);
        board.Columns.Should().Contain(column);
        column.Position.Should().Be(0);
    }

    [Fact]
    public void RemoveColumn_WhenColumnDoesNotExists_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create();

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
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create();
        var workItem = WorkItemFactory.Create();
        var card = BoardCardFactory.Create(column.Id, workItem.Id);
        column.AddCard(card);
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
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create();
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
        var board = BoardFactory.Create();
        var column1 = BoardColumnFactory.Create(name: "Column 1");
        var column2 = BoardColumnFactory.Create(name: "Column 2");
        var column3 = BoardColumnFactory.Create(name: "Column 3");
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
        var board = BoardFactory.Create();
        var column1 = BoardColumnFactory.Create(name: "Column 1");
        var column2 = BoardColumnFactory.Create(name: "Column 2");
        var column3 = BoardColumnFactory.Create(name: "Column 3");
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

    [Fact]
    public void MoveCard_WhenSourceColumnDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create();
        var card = BoardCardFactory.Create(column.Id);
        column.AddCard(card);
        board.AddColumn(column);

        // Act
        var result = board.MoveCard(Guid.NewGuid(), card.Id, column.Id, 0);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public void MoveCard_WhenDestinationColumnDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column1 = BoardColumnFactory.Create();
        var card1 = BoardCardFactory.Create(column1.Id);
        column1.AddCard(card1);
        board.AddColumn(column1);

        // Act
        var result = board.MoveCard(column1.Id, card1.Id, Guid.NewGuid(), 0);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public void MoveCard_WhenCardDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column1 = BoardColumnFactory.Create(board.Id, name: "Column 1");
        var column2 = BoardColumnFactory.Create(board.Id, name: "Column 2");
        board.Columns.Clear();
        board.AddColumn(column1);
        board.AddColumn(column2);

        // Act
        var result = board.MoveCard(column1.Id, Guid.NewGuid(), column2.Id, 0);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.CardNotFound);
    }

    [Fact]
    public void MoveCard_ShouldMoveCardToAnotherColumn()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column1 = BoardColumnFactory.Create(board.Id, name: "Column 1");
        var column2 = BoardColumnFactory.Create(board.Id, name: "Column 2");
        var card = BoardCardFactory.Create(column1.Id);
        board.Columns.Clear();
        board.AddColumn(column1);
        board.AddColumn(column2);
        column1.AddCard(card);

        // Act
        var result = board.MoveCard(column1.Id, card.Id, column2.Id, 0);

        // Assert
        result.IsError.Should().BeFalse();
        column1.Cards.Should().NotContain(card);
        column2.Cards.Should().Contain(card);
        card.BoardColumnId.Should().Be(column2.Id);
    }

    [Fact]
    public void AddCard_WhenColumnDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var board = BoardFactory.Create();
        var card = BoardCardFactory.Create(columnId: Guid.NewGuid());

        // Act
        var result = board.AddCard(card);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.ColumnNotFound);
    }

    [Fact]
    public void AddCard_WhenColumnExists_ShouldAddCard()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create();
        board.AddColumn(column);
        var card = BoardCardFactory.Create(column.Id);

        // Act
        var result = board.AddCard(card);

        // Assert
        result.IsError.Should().BeFalse();
        column.Cards.Should().Contain(card);
    }

    [Fact]
    public void MoveCard_WhenMovingWithinSameColumn_ShouldReorderCard()
    {
        // Arrange
        var board = BoardFactory.Create();
        var column = BoardColumnFactory.Create(board.Id, name: "Column 1");
        var workItem1 = WorkItemFactory.Create();
        var workItem2 = WorkItemFactory.Create();
        var workItem3 = WorkItemFactory.Create();
        var card1 = BoardCardFactory.Create(column.Id, workItem1.Id);
        var card2 = BoardCardFactory.Create(column.Id, workItem2.Id);
        var card3 = BoardCardFactory.Create(column.Id, workItem3.Id);
        board.Columns.Clear();
        board.AddColumn(column);
        column.AddCard(card1);
        column.AddCard(card2);
        column.AddCard(card3);

        // Act
        var result = board.MoveCard(column.Id, card3.Id, column.Id, 2);

        // Assert
        result.IsError.Should().BeFalse();
        card2.Position.Should().Be(0);
        card1.Position.Should().Be(1);
        card3.Position.Should().Be(2);
        column.Cards.Should().HaveCount(3);
    }
}