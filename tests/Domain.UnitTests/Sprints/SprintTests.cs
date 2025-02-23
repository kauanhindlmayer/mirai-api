using Domain.Sprints;
using Domain.UnitTests.WorkItems;

namespace Domain.UnitTests.Sprints;

public class SprintTests
{
    [Fact]
    public void CreateSprint_ShouldSetProperties()
    {
        // Act
        var sprint = SprintFactory.CreateSprint();

        // Assert
        sprint.TeamId.Should().NotBeEmpty();
        sprint.Name.Should().Be(SprintFactory.Name);
        sprint.StartDate.Should().Be(SprintFactory.StartDate);
        sprint.EndDate.Should().Be(SprintFactory.EndDate);
    }

    [Fact]
    public void AddWorkItem_ShouldAddWorkItem()
    {
        // Arrange
        var sprint = SprintFactory.CreateSprint();
        var workItem = WorkItemFactory.CreateWorkItem();

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
        var sprint = SprintFactory.CreateSprint();
        var workItem = WorkItemFactory.CreateWorkItem();
        sprint.AddWorkItem(workItem);

        // Act
        var result = sprint.AddWorkItem(workItem);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.WorkItemAlreadyInSprint);
    }
}