using Domain.UnitTests.WorkItems;

namespace Domain.UnitTests.Boards;

public class BoardCardTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange
        var column = BoardColumnFactory.Create();
        var workItem = WorkItemFactory.Create();

        // Act
        var card = BoardCardFactory.Create(column.Id, workItem.Id);

        // Assert
        card.BoardColumnId.Should().Be(column.Id);
        card.WorkItemId.Should().Be(workItem.Id);
        card.Position.Should().Be(0);
    }

    [Fact]
    public void UpdatePosition_ShouldChangePosition()
    {
        // Arrange
        var column = BoardColumnFactory.Create();
        var workItem = WorkItemFactory.Create();
        var card = BoardCardFactory.Create(column.Id, workItem.Id);

        // Act
        card.UpdatePosition(1);

        // Assert
        card.Position.Should().Be(1);
    }
}