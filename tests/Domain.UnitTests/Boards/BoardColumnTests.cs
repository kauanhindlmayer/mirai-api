using Domain.Boards;
using Domain.UnitTests.WorkItems;

namespace Domain.UnitTests.Boards;

public class BoardColumnTests
{
    [Fact]
    public void AddCard_WhenCardAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();
        var workItem = WorkItemFactory.CreateWorkItem();
        column.AddCard(workItem);

        // Act
        var result = column.AddCard(workItem);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.CardAlreadyExists);
    }

    [Fact]
    public void AddCard_WhenCardDoesNotExists_ShouldAddCard()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();
        var workItem = WorkItemFactory.CreateWorkItem();

        // Act
        var result = column.AddCard(workItem);

        // Assert
        result.IsError.Should().BeFalse();
        column.Cards.Should().HaveCount(1);
        column.Cards.First().WorkItemId.Should().Be(workItem.Id);
    }

    [Fact]
    public void ReorderCards_ShouldUpdateCardPositions()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();
        var workItem1 = WorkItemFactory.CreateWorkItem();
        var workItem2 = WorkItemFactory.CreateWorkItem();
        column.AddCard(workItem1);
        column.AddCard(workItem2);

        // Act
        column.ReorderCards();

        // Assert
        column.Cards.First().Position.Should().Be(0);
        column.Cards.Last().Position.Should().Be(1);
    }

    [Fact]
    public void UpdatePosition_ShouldUpdatePosition()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();

        // Act
        column.UpdatePosition(1);

        // Assert
        column.Position.Should().Be(1);
    }
}