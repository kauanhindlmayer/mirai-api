using Domain.WikiPages;

namespace Domain.UnitTests.WikiPages;

public class WikiPageCommentTests
{
    [Fact]
    public void CreateComment_ShouldSetProperties()
    {
        // Arrange
        var wikiPageId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var content = "Test content";

        // Act
        var comment = new WikiPageComment(wikiPageId, authorId, content);

        // Assert
        comment.WikiPageId.Should().Be(wikiPageId);
        comment.AuthorId.Should().Be(authorId);
        comment.Content.Should().Be(content);
    }

    [Fact]
    public void UpdateContent_ShouldUpdateContent()
    {
        // Arrange
        var wikiPageId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var originalContent = "Original content";
        var newContent = "Updated content";
        var comment = new WikiPageComment(wikiPageId, authorId, originalContent);

        // Act
        comment.UpdateContent(newContent);

        // Assert
        comment.Content.Should().Be(newContent);
    }
}