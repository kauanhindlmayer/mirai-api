using Domain.Retrospectives;
using Domain.Retrospectives.Enums;

namespace Domain.UnitTests.Retrospectives;

public class RetrospectiveTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesAndDefaultColumns()
    {
        // Act
        var retrospective = RetrospectiveData.Create();

        // Assert
        retrospective.Title.Should().Be(RetrospectiveData.Title);
        retrospective.MaxVotesPerUser.Should().Be(RetrospectiveData.MaxVotesPerUser);
        retrospective.Template.Should().Be(RetrospectiveData.Template);
        retrospective.TeamId.Should().Be(RetrospectiveData.TeamId);
        retrospective.Columns.Should().NotBeEmpty();
    }

    [Fact]
    public void Update_ShouldModifyTitleAndMaxVotesPerUser()
    {
        // Arrange
        var retrospective = RetrospectiveData.Create();
        var newTitle = "New Title";
        var newVotes = 10;

        // Act
        retrospective.Update(title: newTitle, maxVotesPerUser: newVotes);

        // Assert
        retrospective.Title.Should().Be(newTitle);
        retrospective.MaxVotesPerUser.Should().Be(newVotes);
    }

    [Fact]
    public void Update_ShouldChangeTemplateAndResetColumns()
    {
        // Arrange
        var retrospective = RetrospectiveData.Create();
        var originalColumnCount = retrospective.Columns.Count;

        // Act
        retrospective.Update(template: RetrospectiveTemplate.Sailboat);

        // Assert
        retrospective.Template.Should().Be(RetrospectiveTemplate.Sailboat);
        retrospective.Columns.Should().NotBeEmpty();
        retrospective.Columns.Count.Should().NotBe(originalColumnCount);
    }

    [Fact]
    public void AddColumn_WhenValid_ShouldAddColumn()
    {
        // Arrange
        var retrospective = RetrospectiveData.Create();
        retrospective.Columns.Clear();
        var column = new RetrospectiveColumn("New Column", retrospective.Id);

        // Act
        var result = retrospective.AddColumn(column);

        // Assert
        result.IsError.Should().BeFalse();
        retrospective.Columns.Should().Contain(column);
    }

    [Fact]
    public void AddColumn_WhenMaxColumnsReached_ShouldReturnError()
    {
        // Arrange
        var retrospective = RetrospectiveData.Create();
        retrospective.Columns.Clear();
        for (int i = 0; i < 5; i++)
        {
            retrospective.AddColumn(new RetrospectiveColumn($"Column {i}", retrospective.Id));
        }

        var extraColumn = new RetrospectiveColumn("Extra Column", retrospective.Id);

        // Act
        var result = retrospective.AddColumn(extraColumn);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.MaxColumnsReached);
    }

    [Fact]
    public void AddColumn_WhenTitleAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var retrospective = RetrospectiveData.Create();
        retrospective.Columns.Clear();
        var column = new RetrospectiveColumn("Duplicate", retrospective.Id);
        retrospective.AddColumn(column);

        // Act
        var result = retrospective.AddColumn(column);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ColumnAlreadyExists);
    }

    [Fact]
    public void AddItem_WhenColumnExists_ShouldAddItem()
    {
        // Arrange
        var retrospective = RetrospectiveData.Create();
        var column = retrospective.Columns.First();
        var item = new RetrospectiveItem(
            "Test Item",
            column.Id,
            RetrospectiveColumnData.AuthorId);

        // Act
        var result = retrospective.AddItem(item);

        // Assert
        result.IsError.Should().BeFalse();
        column.Items.Should().Contain(item);
    }

    [Fact]
    public void AddItem_WhenColumnNotFound_ShouldReturnError()
    {
        // Arrange
        var retrospective = RetrospectiveData.Create();
        var item = new RetrospectiveItem(
            "Test Item",
            Guid.NewGuid(),
            RetrospectiveColumnData.AuthorId);

        // Act
        var result = retrospective.AddItem(item);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ColumnNotFound);
    }

    [Fact]
    public void RemoveItem_WhenItemExists_ShouldRemoveItem()
    {
        // Arrange
        var retrospective = RetrospectiveData.Create();
        var column = retrospective.Columns.First();
        var item = new RetrospectiveItem(
            "Item 1",
            column.Id,
            RetrospectiveColumnData.AuthorId);
        retrospective.AddItem(item);

        // Act
        var result = retrospective.RemoveItem(item.Id);

        // Assert
        result.IsError.Should().BeFalse();
        column.Items.Should().NotContain(item);
    }

    [Fact]
    public void RemoveItem_WhenItemNotFound_ShouldReturnError()
    {
        // Arrange
        var retrospective = RetrospectiveData.Create();

        // Act
        var result = retrospective.RemoveItem(Guid.NewGuid());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ItemNotFound);
    }
}
