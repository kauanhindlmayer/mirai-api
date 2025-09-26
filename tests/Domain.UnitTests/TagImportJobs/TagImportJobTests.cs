using Domain.TagImportJobs;

namespace Domain.UnitTests.TagImportJobs;

public class TagImportJobTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Act
        var job = TagImportJobFactory.Create();

        // Assert
        job.ProjectId.Should().Be(TagImportJobFactory.ProjectId);
        job.FileName.Should().Be(TagImportJobFactory.FileName);
        job.FileContent.Should().BeEquivalentTo(TagImportJobFactory.FileContent);
        job.Status.Should().Be(TagImportJobStatus.Pending);
        job.TotalRecords.Should().Be(0);
        job.ProcessedRecords.Should().Be(0);
        job.SuccessfulRecords.Should().Be(0);
        job.FailedRecords.Should().Be(0);
        job.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetTotalRecords_WithValidNumber_ShouldUpdateTotalRecords()
    {
        // Arrange
        var job = TagImportJobFactory.Create();
        var totalRecords = 100;

        // Act
        job.SetTotalRecords(totalRecords);

        // Assert
        job.TotalRecords.Should().Be(totalRecords);
    }

    [Fact]
    public void IncrementProcessedRecords_ShouldIncreaseByOne()
    {
        // Arrange
        var job = TagImportJobFactory.Create();
        var initialCount = job.ProcessedRecords;

        // Act
        job.IncrementProcessedRecords();

        // Assert
        job.ProcessedRecords.Should().Be(initialCount + 1);
    }

    [Fact]
    public void IncrementSuccessfulRecords_ShouldIncreaseByOne()
    {
        // Arrange
        var job = TagImportJobFactory.Create();
        var initialCount = job.SuccessfulRecords;

        // Act
        job.IncrementSuccessfulRecords();

        // Assert
        job.SuccessfulRecords.Should().Be(initialCount + 1);
    }

    [Fact]
    public void IncrementFailedRecords_ShouldIncreaseByOne()
    {
        // Arrange
        var job = TagImportJobFactory.Create();
        var initialCount = job.FailedRecords;

        // Act
        job.IncrementFailedRecords();

        // Assert
        job.FailedRecords.Should().Be(initialCount + 1);
    }

    [Fact]
    public void StartProcessing_ShouldChangeStatusToProcessing()
    {
        // Arrange
        var job = TagImportJobFactory.Create();

        // Act
        job.StartProcessing();

        // Assert
        job.Status.Should().Be(TagImportJobStatus.Processing);
        job.CompletedAtUtc.Should().BeNull();
    }

    [Fact]
    public void FailProcessing_ShouldSetStatusToFailedAndAddError()
    {
        // Arrange
        var job = TagImportJobFactory.Create();
        var errorMessage = "Processing failed due to invalid format";

        // Act
        job.FailProcessing(errorMessage);

        // Assert
        job.Status.Should().Be(TagImportJobStatus.Failed);
        job.Errors.Should().Contain(errorMessage);
        job.CompletedAtUtc.Should().NotBeNull();
        job.CompletedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CompleteProcessing_ShouldSetStatusToCompletedAndSetCompletedTime()
    {
        // Arrange
        var job = TagImportJobFactory.Create();

        // Act
        job.CompleteProcessing();

        // Assert
        job.Status.Should().Be(TagImportJobStatus.Completed);
        job.CompletedAtUtc.Should().NotBeNull();
        job.CompletedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddError_ShouldAddErrorToErrorsList()
    {
        // Arrange
        var job = TagImportJobFactory.Create();
        var errorMessage = "Line 5: Invalid tag format";

        // Act
        job.AddError(errorMessage);

        // Assert
        job.Errors.Should().Contain(errorMessage);
        job.Errors.Should().HaveCount(1);
    }
}