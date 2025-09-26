using Domain.Sprints;
using Domain.UnitTests.WorkItems;

namespace Domain.UnitTests.Sprints;

public class SprintTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Act
        var sprint = SprintFactory.Create();

        // Assert
        sprint.TeamId.Should().Be(SprintFactory.TeamId);
        sprint.Name.Should().Be(SprintFactory.Name);
        sprint.StartDate.Should().Be(SprintFactory.StartDate);
        sprint.EndDate.Should().Be(SprintFactory.EndDate);
        sprint.WorkItems.Should().BeEmpty();
    }

    [Fact]
    public void AddWorkItem_ShouldAddWorkItem()
    {
        // Arrange
        var sprint = SprintFactory.Create();
        var workItem = WorkItemFactory.Create();

        // Act
        var result = sprint.AddWorkItem(workItem);

        // Assert
        result.IsError.Should().BeFalse();
        sprint.WorkItems.Should().Contain(workItem);
    }

    [Fact]
    public void AddWorkItem_WhenWorkItemAlreadyInSprint_ShouldReturnError()
    {
        // Arrange
        var sprint = SprintFactory.Create();
        var workItem = WorkItemFactory.Create();
        sprint.AddWorkItem(workItem);

        // Act
        var result = sprint.AddWorkItem(workItem);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.WorkItemAlreadyInSprint);
    }
}