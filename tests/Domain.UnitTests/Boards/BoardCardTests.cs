using Domain.UnitTests.WorkItems;

namespace Domain.UnitTests.Boards;

public class BoardCardTests
{
    [Fact]
    public void CreateBoardCard_ShouldSetProperties()
    {
        // Arrange
        var column = BoardFactory.CreateBoardColumn();
        var workItem = WorkItemFactory.CreateWorkItem();

        // Act
        var card = BoardFactory.CreateBoardCard(column.Id, workItem.Id);

        // Assert
        card.BoardColumnId.Should().Be(column.Id);
        card.WorkItemId.Should().Be(workItem.Id);
        card.Position.Should().Be(0);
    }
}