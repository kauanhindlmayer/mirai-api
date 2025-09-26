using Domain.Boards;
using Domain.UnitTests.WorkItems;
using ErrorOr;

namespace Domain.UnitTests.Boards;

public class BoardColumnTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Act
        var column = BoardColumnFactory.Create();

        // Assert
        column.BoardId.Should().Be(BoardColumnFactory.BoardId);
        column.Name.Should().Be(BoardColumnFactory.Name);
        column.WipLimit.Should().Be(BoardColumnFactory.WipLimit);
        column.DefinitionOfDone.Should().Be(BoardColumnFactory.DefinitionOfDone);
    }

    [Fact]
    public void UpdatePosition_ShouldChangePosition()
    {
        // Arrange
        var column = BoardColumnFactory.Create();

        // Act
        column.UpdatePosition(1);

        // Assert
        column.Position.Should().Be(1);
    }

    [Fact]
    public void AddCard_WhenCardIsValid_ShouldAddCard()
    {
        // Arrange
        var column = BoardColumnFactory.Create();
        var workItem = WorkItemFactory.Create();
        var card = BoardCardFactory.Create(column.Id, workItem.Id);

        // Act
        var result = column.AddCard(card);

        // Assert
        result.IsError.Should().BeFalse();
        column.Cards.Should().Contain(card);
    }

    [Fact]
    public void AddCard_WhenCardWithSameWorkItemExists_ShouldReturnError()
    {
        // Arrange
        var column = BoardColumnFactory.Create();
        var workItem = WorkItemFactory.Create();
        var card = BoardCardFactory.Create(column.Id, workItem.Id);
        column.AddCard(card);

        // Act
        var result = column.AddCard(card);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.CardAlreadyExists);
    }

    [Fact]
    public void RemoveCard_ShouldShiftCardsLeft()
    {
        // Arrange
        var column = BoardColumnFactory.Create();
        var workItem1 = WorkItemFactory.Create();
        var workItem2 = WorkItemFactory.Create();
        var workItem3 = WorkItemFactory.Create();
        var card1 = BoardCardFactory.Create(column.Id, workItem1.Id);
        var card2 = BoardCardFactory.Create(column.Id, workItem2.Id);
        var card3 = BoardCardFactory.Create(column.Id, workItem3.Id);
        column.AddCard(card1);
        column.AddCard(card2);
        column.AddCard(card3);

        // Act
        var result = column.RemoveCard(card2.Id);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(card2);
        column.Cards.Should().NotContain(card2);
        card1.Position.Should().Be(1);
    }

    [Fact]
    public void RemoveCard_WhenCardExists_ShouldRemoveCard()
    {
        // Arrange
        var column = BoardColumnFactory.Create();
        var workItem = WorkItemFactory.Create();
        var card = BoardCardFactory.Create(column.Id, workItem.Id);
        column.AddCard(card);

        // Act
        var result = column.RemoveCard(card.Id);

        // Assert
        result.IsError.Should().BeFalse();
        column.Cards.Should().NotContain(card);
    }

    [Fact]
    public void RemoveCard_WhenCardDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var column = BoardColumnFactory.Create();
        var workItem = WorkItemFactory.Create();
        var card = BoardCardFactory.Create(column.Id, workItem.Id);

        // Act
        var result = column.RemoveCard(card.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.CardNotFound);
    }

    [Fact]
    public void AddCardAtPosition_ShouldAddCardAtGivenPositionAndShiftOthers()
    {
        // Arrange
        var column = BoardColumnFactory.Create();
        var workItem1 = WorkItemFactory.Create();
        var workItem2 = WorkItemFactory.Create();
        var workItem3 = WorkItemFactory.Create();
        var card1 = BoardCardFactory.Create(column.Id, workItem1.Id);
        var card2 = BoardCardFactory.Create(column.Id, workItem2.Id);
        var card3 = BoardCardFactory.Create(column.Id, workItem3.Id);
        column.AddCard(card1);
        column.AddCard(card2);

        // Act
        var result = column.AddCardAtPosition(card3, 1);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
        card2.Position.Should().Be(0);
        card3.Position.Should().Be(1);
        card1.Position.Should().Be(2);
    }

    [Fact]
    public void AddCardAtPosition_WhenPositionIsInvalid_ShouldReturnError()
    {
        // Arrange
        var column = BoardColumnFactory.Create();
        var card1 = BoardCardFactory.Create();
        column.AddCard(card1);
        var card2 = BoardCardFactory.Create();

        // Act
        var result = column.AddCardAtPosition(card2, -1);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.InvalidPosition);
    }

    [Fact]
    public void AddCardAtPosition_WhenCardAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var column = BoardColumnFactory.Create();
        var card1 = BoardCardFactory.Create();
        var card2 = BoardCardFactory.Create();
        column.AddCard(card1);
        column.AddCard(card2);

        // Act
        var result = column.AddCardAtPosition(card1, 1);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(BoardErrors.CardAlreadyExists);
    }
}