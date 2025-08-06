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
        var card = BoardFactory.CreateBoardCard(column.Id, workItem.Id);
        column.AddCard(card);

        // Act
        var result = column.AddCard(card);

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
        var card = BoardFactory.CreateBoardCard(column.Id, workItem.Id);

        // Act
        var result = column.AddCard(card);

        // Assert
        result.IsError.Should().BeFalse();
        column.Cards.Should().HaveCount(1);
        column.Cards.First().WorkItemId.Should().Be(workItem.Id);
    }

    // [Fact]
    // public void ReorderCards_ShouldUpdateCardPositions()
    // {
    //     // Arrange
    //     var column = BoardFactory.CreateBoardColumn();
    //     var workItem1 = WorkItemFactory.CreateWorkItem();
    //     var workItem2 = WorkItemFactory.CreateWorkItem();
    //     var card1 = new BoardCard(column.Id, workItem1.Id, 0);
    //     var card2 = new BoardCard(column.Id, workItem2.Id, 0);
    //     column.AddCard(card1);
    //     column.AddCard(card2);

    //     // Act
    //     column.ReorderCards();

    //     // Assert
    //     column.Cards.First().Position.Should().Be(0);
    //     column.Cards.Last().Position.Should().Be(1);
    // }

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

    [Fact]
    public void RemoveCard_WhenCardDoesNotExists_ShouldReturnError()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();
        var workItem = WorkItemFactory.CreateWorkItem();

        // Act
        var result = column.RemoveCard(workItem.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.CardNotFound);
    }

    [Fact]
    public void RemoveCard_WhenCardExists_ShouldRemoveCard()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();
        var workItem = WorkItemFactory.CreateWorkItem();
        var card = BoardFactory.CreateBoardCard(column.Id, workItem.Id);
        column.AddCard(card);

        // Act
        var result = column.RemoveCard(card.Id);

        // Assert
        result.IsError.Should().BeFalse();
        column.Cards.Should().BeEmpty();
    }

    [Fact]
    public void AddCardAtPosition_WhenPositionIsInvalid_ShouldReturnError()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();
        var workItem = WorkItemFactory.CreateWorkItem();
        var card = BoardFactory.CreateBoardCard(column.Id, workItem.Id);

        // Act
        var result = column.AddCardAtPosition(card, 2);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.InvalidPosition);
    }

    [Fact]
    public void AddCardAtPosition_WhenCardAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();
        var workItem = WorkItemFactory.CreateWorkItem();
        var card = BoardFactory.CreateBoardCard(column.Id, workItem.Id);
        column.AddCard(card);

        // Act
        var result = column.AddCardAtPosition(card, 0);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.CardAlreadyExists);
    }

    [Fact]
    public void AddCardAtPosition_WhenPositionIsValid_ShouldAddCard()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();
        var workItem = WorkItemFactory.CreateWorkItem();
        var card = new BoardCard(column.Id, workItem.Id, 0);

        // Act
        var result = column.AddCardAtPosition(card, 0);

        // Assert
        result.IsError.Should().BeFalse();
        column.Cards.Should().HaveCount(1);
        column.Cards.First().WorkItemId.Should().Be(workItem.Id);
    }

    [Fact]
    public void AddCardAtPosition_WhenPositionIsValid_ShouldReorderCards()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();
        var workItem1 = WorkItemFactory.CreateWorkItem();
        var card1 = BoardFactory.CreateBoardCard(column.Id, workItem1.Id);
        column.AddCard(card1);
        var workItem2 = WorkItemFactory.CreateWorkItem();
        var card2 = BoardFactory.CreateBoardCard(column.Id, workItem2.Id);

        // Act
        column.AddCardAtPosition(card2, 0);

        // Assert
        card1.Position.Should().Be(1);
        card2.Position.Should().Be(0);
    }
}