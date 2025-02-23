using Domain.Retrospectives;

namespace Domain.UnitTests.Retrospectives;

public class RetrospectiveColumnTests
{
    [Fact]
    public void CreateColumn_ShouldSetProperties()
    {
        // Act
        var column = RetrospectiveFactory.CreateColumn();

        // Assert
        column.Title.Should().NotBeNullOrEmpty();
        column.Position.Should().Be(0);
        column.RetrospectiveId.Should().NotBeEmpty();
    }

    [Fact]
    public void UpdatePosition_ShouldSetPosition()
    {
        // Arrange
        var column = RetrospectiveFactory.CreateColumn();
        var position = 1;

        // Act
        column.UpdatePosition(position);

        // Assert
        column.Position.Should().Be(position);
    }

    [Fact]
    public void AddItem_ShouldAddItem()
    {
        // Arrange
        var column = RetrospectiveFactory.CreateColumn();
        var item = RetrospectiveFactory.CreateItem();

        // Act
        var result = column.AddItem(item);

        // Assert
        result.IsError.Should().BeFalse();
        column.Items.Should().Contain(item);
    }

    [Fact]
    public void AddItem_WhenItemAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var column = RetrospectiveFactory.CreateColumn();
        var item = RetrospectiveFactory.CreateItem();
        column.AddItem(item);

        // Act
        var result = column.AddItem(item);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ItemAlreadyExists);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItem()
    {
        // Arrange
        var column = RetrospectiveFactory.CreateColumn();
        var item = RetrospectiveFactory.CreateItem();
        column.AddItem(item);

        // Act
        var result = column.RemoveItem(item.Id);

        // Assert
        result.IsError.Should().BeFalse();
        column.Items.Should().NotContain(item);
    }

    [Fact]
    public void RemoveItem_WhenItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var column = RetrospectiveFactory.CreateColumn();
        var item = RetrospectiveFactory.CreateItem();

        // Act
        var result = column.RemoveItem(item.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ItemNotFound);
    }
}