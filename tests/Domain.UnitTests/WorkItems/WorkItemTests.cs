using Domain.Tags;
using Domain.UnitTests.Tags;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Pgvector;

namespace Domain.UnitTests.WorkItems;

public class WorkItemTests
{
    [Fact]
    public void Assign_ShouldSetAssigneeId()
    {
        // Arrange
        var workItem = WorkItemFactory.CreateWorkItem();
        var assigneeId = Guid.NewGuid();

        // Act
        workItem.Assign(assigneeId);

        // Assert
        workItem.AssignedUserId.Should().Be(assigneeId);
    }

    [Fact]
    public void Close_ShouldSetStatusToClosed()
    {
        // Arrange
        var workItem = WorkItemFactory.CreateWorkItem();

        // Act
        workItem.Close();

        // Assert
        workItem.Status.Should().Be(WorkItemStatus.Closed);
    }

    [Fact]
    public void SetSearchVector_ShouldSetSearchVector()
    {
        // Arrange
        var workItem = WorkItemFactory.CreateWorkItem();
        float[] searchEmbedding = [1.0f, 2.0f, 3.0f];

        // Act
        workItem.SetSearchVector(searchEmbedding);

        // Assert
        workItem.SearchVector.Should().Be(new Vector(searchEmbedding));
    }

    [Fact]
    public void AddComment_WhenWorkItemIsClosed_ShouldReturnError()
    {
        // Arrange
        var workItem = WorkItemFactory.CreateWorkItem();
        workItem.Close();
        var comment = new WorkItemComment(workItem.Id, Guid.NewGuid(), "Comment");

        // Act
        var result = workItem.AddComment(comment);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.CannotAddCommentToClosedWorkItem);
    }

    [Fact]
    public void AddComment_WhenWorkItemIsNotClosed_ShouldAddComment()
    {
        // Arrange
        var workItem = WorkItemFactory.CreateWorkItem();
        var comment = new WorkItemComment(workItem.Id, Guid.NewGuid(), "Comment");

        // Act
        var result = workItem.AddComment(comment);

        // Assert
        result.IsError.Should().BeFalse();
        workItem.Comments.Should().Contain(comment);
    }

    [Fact]
    public void RemoveComment_WhenCommentDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var workItem = WorkItemFactory.CreateWorkItem();
        var comment = new WorkItemComment(workItem.Id, Guid.NewGuid(), "Comment");

        // Act
        var result = workItem.RemoveComment(comment.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.CommentNotFound);
    }

    [Fact]
    public void RemoveComment_WhenCommentExists_ShouldRemoveComment()
    {
        // Arrange
        var workItem = WorkItemFactory.CreateWorkItem();
        var comment = new WorkItemComment(workItem.Id, Guid.NewGuid(), "Comment");
        workItem.AddComment(comment);

        // Act
        var result = workItem.RemoveComment(comment.Id);

        // Assert
        result.IsError.Should().BeFalse();
        workItem.Comments.Should().NotContain(comment);
    }

    [Fact]
    public void AddTag_ShouldAddTag()
    {
        // Arrange
        var workItem = WorkItemFactory.CreateWorkItem();
        var tag = TagFactory.CreateTag();

        // Act
        workItem.AddTag(tag);

        // Assert
        workItem.Tags.Should().Contain(tag);
    }

    [Fact]
    public void RemoveTag_WhenTagDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var workItem = WorkItemFactory.CreateWorkItem();
        var tag = TagFactory.CreateTag();

        // Act
        var result = workItem.RemoveTag(tag.Name);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TagErrors.NotFound);
    }

    [Fact]
    public void RemoveTag_WhenTagExists_ShouldRemoveTag()
    {
        // Arrange
        var workItem = WorkItemFactory.CreateWorkItem();
        var tag = TagFactory.CreateTag();
        workItem.AddTag(tag);

        // Act
        var result = workItem.RemoveTag(tag.Name);

        // Assert
        result.IsError.Should().BeFalse();
        workItem.Tags.Should().NotContain(tag);
    }
}
