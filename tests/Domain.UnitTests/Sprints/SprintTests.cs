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

    [Fact]
    public void Update_ShouldOverwriteNameAndDates()
    {
        // Arrange
        var sprint = SprintFactory.Create();
        var startDate = new DateOnly(2026, 3, 2);
        var endDate = new DateOnly(2026, 3, 13);

        // Act
        sprint.Update("Sprint 42", startDate, endDate);

        // Assert
        sprint.Name.Should().Be("Sprint 42");
        sprint.StartDate.Should().Be(startDate);
        sprint.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void ReturnWorkItemsToBacklog_ShouldDetachEveryWorkItem()
    {
        // Arrange
        var sprint = SprintFactory.Create();
        var workItem = WorkItemFactory.Create();
        sprint.AddWorkItem(workItem);

        // Act
        sprint.ReturnWorkItemsToBacklog();

        // Assert
        sprint.WorkItems.Should().BeEmpty();
        workItem.SprintId.Should().BeNull();
    }

    [Theory]
    [InlineData(1, 14, true)]
    [InlineData(8, 21, true)]
    [InlineData(1, 7, true)]
    [InlineData(14, 28, true)]
    [InlineData(15, 28, false)]
    [InlineData(16, 28, false)]
    public void Overlaps_ShouldTreatBothEndpointsAsInclusive(
        int startDay,
        int endDay,
        bool expected)
    {
        // Arrange
        var sprint = SprintFactory.Create(
            startDate: new DateOnly(2026, 6, 1),
            endDate: new DateOnly(2026, 6, 14));

        // Act
        var result = sprint.Overlaps(
            new DateOnly(2026, 6, startDay),
            new DateOnly(2026, 6, endDay));

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Constructor_ShouldStartOutPlanned()
    {
        // Act
        var sprint = SprintFactory.Create();

        // Assert
        sprint.Status.Should().Be(SprintStatus.Planned);
        sprint.StartedAtUtc.Should().BeNull();
    }

    [Fact]
    public void Start_ShouldMakeTheSprintActiveAndRecordWhen()
    {
        // Arrange
        var sprint = SprintFactory.Create();

        // Act
        sprint.Start();

        // Assert
        sprint.Status.Should().Be(SprintStatus.Active);
        sprint.StartedAtUtc.Should().NotBeNull();
    }
}
