using Domain.Sprints;
using Domain.UnitTests.WorkItems;
using Domain.WorkItems.ValueObjects;

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

    [Fact]
    public void Start_ShouldSnapshotOneCommitmentPerWorkItem()
    {
        // Arrange
        var sprint = SprintFactory.Create();
        var estimated = WorkItemFactory.Create();
        estimated.Update(planning: new Planning { StoryPoints = 5 });
        var unestimated = WorkItemFactory.Create();
        sprint.AddWorkItem(estimated);
        sprint.AddWorkItem(unestimated);

        // Act
        sprint.Start();

        // Assert
        sprint.Commitments.Should().HaveCount(2);
        sprint.Commitments.Should().ContainSingle(c =>
            c.WorkItemId == estimated.Id
            && c.CommittedStoryPoints == 5
            && c.StatusAtStart == estimated.Status);
        sprint.Commitments.Should().ContainSingle(c =>
            c.WorkItemId == unestimated.Id && c.CommittedStoryPoints == null);
    }

    [Fact]
    public void Start_WhenSprintIsEmpty_ShouldRecordNoCommitments()
    {
        // Arrange
        var sprint = SprintFactory.Create();

        // Act
        sprint.Start();

        // Assert
        sprint.Commitments.Should().BeEmpty();
        sprint.CommittedStoryPoints.Should().Be(0);
        sprint.CommittedWorkItemCount.Should().Be(0);
    }

    [Fact]
    public void CommittedFigures_ShouldSumCommittedPointsAndCountItems()
    {
        // Arrange
        var sprint = SprintFactory.Create();
        var first = WorkItemFactory.Create();
        first.Update(planning: new Planning { StoryPoints = 3 });
        var second = WorkItemFactory.Create();
        second.Update(planning: new Planning { StoryPoints = 8 });
        var unestimated = WorkItemFactory.Create();
        sprint.AddWorkItem(first);
        sprint.AddWorkItem(second);
        sprint.AddWorkItem(unestimated);

        // Act
        sprint.Start();

        // Assert
        sprint.CommittedStoryPoints.Should().Be(11);
        sprint.CommittedWorkItemCount.Should().Be(3);
    }

    [Fact]
    public void Commitments_ShouldNotChangeWhenAWorkItemIsReEstimatedAfterStart()
    {
        // Arrange
        var sprint = SprintFactory.Create();
        var workItem = WorkItemFactory.Create();
        workItem.Update(planning: new Planning { StoryPoints = 5 });
        sprint.AddWorkItem(workItem);
        sprint.Start();

        // Act
        workItem.Update(planning: new Planning { StoryPoints = 13 });

        // Assert
        sprint.CommittedStoryPoints.Should().Be(5);
    }
}
