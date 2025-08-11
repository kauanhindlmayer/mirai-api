using Domain.Retrospectives;

namespace Domain.UnitTests.Retrospectives;

public class RetrospectiveColumnTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Act
        var column = RetrospectiveColumnData.Create();

        // Assert
        column.Title.Should().Be(RetrospectiveColumnData.Title);
        column.RetrospectiveId.Should().Be(RetrospectiveColumnData.RetrospectiveId);
        column.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_WhenUnique_ShouldAddAtPositionZeroAndShiftOthers()
    {
        // Arrange
        var column = RetrospectiveColumnData.Create();
        var existingItem = RetrospectiveColumnData.CreateItem("Existing Item", column.Id, position: 0);
        column.Items.Add(existingItem);

        var newItem = RetrospectiveColumnData.CreateItem("New Item", column.Id);

        // Act
        var result = column.AddItem(newItem);

        // Assert
        result.IsError.Should().BeFalse();
        newItem.Position.Should().Be(0);
        existingItem.Position.Should().Be(1); // shifted
        column.Items.Should().Contain(newItem);
    }

    [Fact]
    public void AddItem_WhenContentAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var column = RetrospectiveColumnData.Create();
        var item = RetrospectiveColumnData.CreateItem("Duplicate", column.Id);
        column.Items.Add(item);

        var duplicateItem = RetrospectiveColumnData.CreateItem("Duplicate", column.Id);

        // Act
        var result = column.AddItem(duplicateItem);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ItemAlreadyExists);
        column.Items.Should().NotContain(duplicateItem);
    }

    [Fact]
    public void RemoveItem_WhenExists_ShouldRemoveAndShiftPositions()
    {
        // Arrange
        var column = RetrospectiveColumnData.Create();
        var item1 = RetrospectiveColumnData.CreateItem("Item 1", column.Id, position: 0);
        var item2 = RetrospectiveColumnData.CreateItem("Item 2", column.Id, position: 1);
        column.Items.AddRange([item1, item2]);

        // Act
        var result = column.RemoveItem(item1.Id);

        // Assert
        result.IsError.Should().BeFalse();
        column.Items.Should().NotContain(item1);
        item2.Position.Should().Be(0); // shifted
    }

    [Fact]
    public void RemoveItem_WhenNotFound_ShouldReturnError()
    {
        // Arrange
        var column = RetrospectiveColumnData.Create();

        // Act
        var result = column.RemoveItem(Guid.NewGuid());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ItemNotFound);
    }

    [Fact]
    public void UpdatePosition_ShouldChangePosition()
    {
        // Arrange
        var column = RetrospectiveColumnData.Create();

        // Act
        column.UpdatePosition(3);

        // Assert
        column.Position.Should().Be(3);
    }
}
