using Domain.Sprints;
using Domain.UnitTests.WorkItems;

namespace Domain.UnitTests.Sprints;

public class SprintTests
{
    [Fact]
    public void CreateSprint_ShouldSetProperties()
    {
        // Act
        var sprint = SprintData.Create();

        // Assert
        sprint.TeamId.Should().Be(SprintData.TeamId);
        sprint.Name.Should().Be(SprintData.Name);
        sprint.StartDate.Should().Be(SprintData.StartDate);
        sprint.EndDate.Should().Be(SprintData.EndDate);
        sprint.WorkItems.Should().BeEmpty();
    }

    [Fact]
    public void AddWorkItem_ShouldAddWorkItem()
    {
        // Arrange
        var sprint = SprintData.Create();
        var workItem = WorkItemData.Create();

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
        var sprint = SprintData.Create();
        var workItem = WorkItemData.Create();
        sprint.AddWorkItem(workItem);

        // Act
        var result = sprint.AddWorkItem(workItem);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.WorkItemAlreadyInSprint);
    }
}