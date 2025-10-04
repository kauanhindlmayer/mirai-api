using Domain.Tags;
using Domain.UnitTests.Tags;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Pgvector;

namespace Domain.UnitTests.WorkItems;

public class WorkItemTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Act
        var workItem = WorkItemFactory.Create();

        // Assert
        workItem.ProjectId.Should().Be(WorkItemFactory.ProjectId);
        workItem.Code.Should().Be(WorkItemFactory.Code);
        workItem.Title.Should().Be(WorkItemFactory.Title);
        workItem.Type.Should().Be(WorkItemFactory.Type);
        workItem.Status.Should().Be(WorkItemStatus.New);
        workItem.AssignedTeamId.Should().Be(WorkItemFactory.AssignedTeamId);
        workItem.SprintId.Should().Be(WorkItemFactory.SprintId);
        workItem.ParentWorkItemId.Should().Be(WorkItemFactory.ParentWorkItemId);
        workItem.AssigneeId.Should().BeNull();
        workItem.Description.Should().BeNull();
        workItem.AcceptanceCriteria.Should().BeNull();
        workItem.CompletedAtUtc.Should().BeNull();
    }

    [Fact]
    public void Assign_WithValidUserId_ShouldSetAssigneeId()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var userId = Guid.NewGuid();

        // Act
        workItem.UpdateAssignment(userId);

        // Assert
        workItem.AssigneeId.Should().Be(userId);
    }

    [Fact]
    public void Close_ShouldSetStatusToClosed()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();

        // Act
        workItem.Close();

        // Assert
        workItem.Status.Should().Be(WorkItemStatus.Closed);
    }

    [Fact]
    public void Update_WithNewTitle_ShouldUpdateTitle()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var newTitle = "Updated Title";

        // Act
        workItem.Update(title: newTitle);

        // Assert
        workItem.Title.Should().Be(newTitle);
        // Other properties should remain unchanged
        workItem.Code.Should().Be(WorkItemFactory.Code);
        workItem.Type.Should().Be(WorkItemFactory.Type);
    }

    [Fact]
    public void Update_WithMultipleParameters_ShouldUpdateAllSpecifiedFields()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var newTitle = "Updated Title";
        var newDescription = "Updated Description";
        var newStatus = WorkItemStatus.Active;
        var newAssigneeId = Guid.NewGuid();

        // Act
        workItem.Update(
            title: newTitle,
            description: newDescription,
            status: newStatus);
        workItem.UpdateAssignment(newAssigneeId);

        // Assert
        workItem.Title.Should().Be(newTitle);
        workItem.Description.Should().Be(newDescription);
        workItem.Status.Should().Be(newStatus);
        workItem.AssigneeId.Should().Be(newAssigneeId);
    }

    [Fact]
    public void Update_WithNullValues_ShouldKeepExistingValues()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var originalTitle = workItem.Title;
        var originalType = workItem.Type;

        // Act
        workItem.Update(title: null, type: null);

        // Assert
        workItem.Title.Should().Be(originalTitle);
        workItem.Type.Should().Be(originalType);
    }

    [Fact]
    public void AddComment_WithValidComment_ShouldAddCommentSuccessfully()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var comment = WorkItemCommentFactory.Create(workItem.Id);

        // Act
        var result = workItem.AddComment(comment);

        // Assert
        result.IsError.Should().BeFalse();
        workItem.Comments.Should().Contain(comment);
        workItem.Comments.Should().HaveCount(1);
    }

    [Fact]
    public void AddComment_WhenWorkItemIsClosed_ShouldReturnError()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        workItem.Close();
        var comment = WorkItemCommentFactory.Create(workItem.Id);

        // Act
        var result = workItem.AddComment(comment);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.CannotAddCommentToClosedWorkItem);
        workItem.Comments.Should().BeEmpty();
    }

    [Fact]
    public void RemoveComment_WithExistingComment_ShouldRemoveSuccessfully()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var comment = WorkItemCommentFactory.Create(workItem.Id);
        workItem.AddComment(comment);

        // Act
        var result = workItem.RemoveComment(comment.Id);

        // Assert
        result.IsError.Should().BeFalse();
        workItem.Comments.Should().NotContain(comment);
        workItem.Comments.Should().BeEmpty();
    }

    [Fact]
    public void RemoveComment_WithNonExistentComment_ShouldReturnError()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var nonExistentCommentId = Guid.NewGuid();

        // Act
        var result = workItem.RemoveComment(nonExistentCommentId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.CommentNotFound);
    }

    [Fact]
    public void UpdateComment_WithValidCommentAndAuthor_ShouldUpdateSuccessfully()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var comment = WorkItemCommentFactory.Create(workItem.Id);
        workItem.AddComment(comment);
        var newContent = "Updated content";

        // Act
        var result = workItem.UpdateComment(comment.Id, newContent, comment.AuthorId);

        // Assert
        result.IsError.Should().BeFalse();
        comment.Content.Should().Be(newContent);
    }

    [Fact]
    public void UpdateComment_WithNonExistentComment_ShouldReturnCommentNotFound()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var commentId = Guid.NewGuid();

        // Act
        var result = workItem.UpdateComment(commentId, "Updated content", Guid.NewGuid());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.CommentNotFound);
    }

    [Fact]
    public void UpdateComment_WithWrongAuthor_ShouldReturnCommentNotOwned()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var comment = WorkItemCommentFactory.Create(workItem.Id);
        workItem.AddComment(comment);
        var wrongUserId = Guid.NewGuid();

        // Act
        var result = workItem.UpdateComment(comment.Id, "Updated content", wrongUserId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.CommentNotOwned);
        comment.Content.Should().NotBe("Updated content");
    }

    [Fact]
    public void AddTag_WithValidTag_ShouldAddTag()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var tag = TagFactory.Create();

        // Act
        workItem.AddTag(tag);

        // Assert
        workItem.Tags.Should().Contain(tag);
        workItem.Tags.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveTag_WithExistingTag_ShouldRemoveSuccessfully()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var tag = TagFactory.Create();
        workItem.AddTag(tag);

        // Act
        var result = workItem.RemoveTag(tag.Name);

        // Assert
        result.IsError.Should().BeFalse();
        workItem.Tags.Should().NotContain(tag);
        workItem.Tags.Should().BeEmpty();
    }

    [Fact]
    public void RemoveTag_WithNonExistentTag_ShouldReturnError()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var nonExistentTagName = "non-existent-tag";

        // Act
        var result = workItem.RemoveTag(nonExistentTagName);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TagErrors.NotFound);
    }

    [Fact]
    public void SetSearchVector_WithValidEmbedding_ShouldSetSearchVector()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var embedding = new float[] { 0.1f, 0.2f, 0.3f };

        // Act
        workItem.SetSearchVector(embedding);

        // Assert
        workItem.SearchVector.Should().BeEquivalentTo(new Vector(embedding));
    }

    [Fact]
    public void GetEmbeddingContent_WithAllProperties_ShouldReturnFormattedContent()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        workItem.Update(
            description: "Test description",
            acceptanceCriteria: "Test criteria");

        // Act
        var content = workItem.GetEmbeddingContent();

        // Assert
        content.Should().Contain($"Title: {WorkItemFactory.Title}");
        content.Should().Contain($"Type: {WorkItemFactory.Type}");
        content.Should().Contain("Status: New");
        content.Should().Contain("Description: Test description");
        content.Should().Contain("Acceptance Criteria: Test criteria");
    }

    [Fact]
    public void GetEmbeddingContent_WithMinimalProperties_ShouldReturnBasicContent()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();

        // Act
        var content = workItem.GetEmbeddingContent();

        // Assert
        content.Should().Contain($"Title: {WorkItemFactory.Title}");
        content.Should().Contain($"Type: {WorkItemFactory.Type}");
        content.Should().Contain("Status: New");
        content.Should().NotContain("Description:");
        content.Should().NotContain("Acceptance Criteria:");
    }

    [Fact]
    public void AddLink_WithValidLink_ShouldAddLinkSuccessfully()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var targetWorkItemId = Guid.NewGuid();
        var link = WorkItemLinkFactory.Create(
            sourceWorkItemId: workItem.Id,
            targetWorkItemId: targetWorkItemId);

        // Act
        var result = workItem.AddLink(link);

        // Assert
        result.IsError.Should().BeFalse();
        workItem.OutgoingLinks.Should().Contain(link);
        workItem.OutgoingLinks.Should().HaveCount(1);
    }

    [Fact]
    public void AddLink_WithSelfLink_ShouldReturnError()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var link = WorkItemLinkFactory.Create(
            sourceWorkItemId: workItem.Id,
            targetWorkItemId: workItem.Id);

        // Act
        var result = workItem.AddLink(link);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.CannotLinkToSelf);
        workItem.OutgoingLinks.Should().BeEmpty();
    }

    [Fact]
    public void AddLink_WithDuplicateLink_ShouldReturnError()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var targetWorkItemId = Guid.NewGuid();
        var link1 = WorkItemLinkFactory.Create(
            sourceWorkItemId: workItem.Id,
            targetWorkItemId: targetWorkItemId,
            linkType: WorkItemLinkType.Related);
        var link2 = WorkItemLinkFactory.Create(
            sourceWorkItemId: workItem.Id,
            targetWorkItemId: targetWorkItemId,
            linkType: WorkItemLinkType.Related);

        workItem.AddLink(link1);

        // Act
        var result = workItem.AddLink(link2);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.LinkAlreadyExists);
        workItem.OutgoingLinks.Should().HaveCount(1);
    }

    [Fact]
    public void AddLink_WithSameTargetButDifferentLinkType_ShouldAddBothLinks()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var targetWorkItemId = Guid.NewGuid();
        var link1 = WorkItemLinkFactory.Create(
            sourceWorkItemId: workItem.Id,
            targetWorkItemId: targetWorkItemId,
            linkType: WorkItemLinkType.Related);
        var link2 = WorkItemLinkFactory.Create(
            sourceWorkItemId: workItem.Id,
            targetWorkItemId: targetWorkItemId,
            linkType: WorkItemLinkType.Duplicate);

        // Act
        var result1 = workItem.AddLink(link1);
        var result2 = workItem.AddLink(link2);

        // Assert
        result1.IsError.Should().BeFalse();
        result2.IsError.Should().BeFalse();
        workItem.OutgoingLinks.Should().HaveCount(2);
        workItem.OutgoingLinks.Should().Contain(link1);
        workItem.OutgoingLinks.Should().Contain(link2);
    }

    [Fact]
    public void RemoveLink_WithExistingLink_ShouldRemoveSuccessfully()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var targetWorkItemId = Guid.NewGuid();
        var link = WorkItemLinkFactory.Create(
            sourceWorkItemId: workItem.Id,
            targetWorkItemId: targetWorkItemId);
        workItem.AddLink(link);

        // Act
        var result = workItem.RemoveLink(link.Id);

        // Assert
        result.IsError.Should().BeFalse();
        workItem.OutgoingLinks.Should().NotContain(link);
        workItem.OutgoingLinks.Should().BeEmpty();
    }

    [Fact]
    public void RemoveLink_WithNonExistentLink_ShouldReturnError()
    {
        // Arrange
        var workItem = WorkItemFactory.Create();
        var nonExistentLinkId = Guid.NewGuid();

        // Act
        var result = workItem.RemoveLink(nonExistentLinkId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.LinkNotFound);
    }
}