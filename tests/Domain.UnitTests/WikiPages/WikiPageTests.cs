using Domain.WikiPages;

namespace Domain.UnitTests.WikiPages;

public class WikiPageTests
{
    [Fact]
    public void CreateWikiPage_ShouldSetProperties()
    {
        // Act
        var wikiPage = WikiPageFactory.CreateWikiPage();

        // Assert
        wikiPage.Title.Should().Be(WikiPageFactory.Title);
        wikiPage.Content.Should().Be(WikiPageFactory.Content);
    }

    [Fact]
    public void Update_ShouldUpdateProperties()
    {
        // Arrange
        var wikiPage = WikiPageFactory.CreateWikiPage();
        var title = "New Title";
        var content = "New Content";

        // Act
        wikiPage.Update(title, content);

        // Assert
        wikiPage.Title.Should().Be(title);
        wikiPage.Content.Should().Be(content);
    }

    [Fact]
    public void UpdatePosition_ShouldUpdatePosition()
    {
        // Arrange
        var wikiPage = WikiPageFactory.CreateWikiPage();
        var position = 1;

        // Act
        wikiPage.UpdatePosition(position);

        // Assert
        wikiPage.Position.Should().Be(position);
    }

    [Fact]
    public void AddComment_ShouldAddComment()
    {
        // Arrange
        var wikiPage = WikiPageFactory.CreateWikiPage();
        var comment = new WikiPageComment(wikiPage.Id, Guid.NewGuid(), "Content");

        // Act
        wikiPage.AddComment(comment);

        // Assert
        wikiPage.Comments.Should().Contain(comment);
    }

    [Fact]
    public void RemoveComment_WhenCommentExists_ShouldRemoveComment()
    {
        // Arrange
        var wikiPage = WikiPageFactory.CreateWikiPage();
        var comment = new WikiPageComment(wikiPage.Id, Guid.NewGuid(), "Content");
        wikiPage.AddComment(comment);

        // Act
        var result = wikiPage.RemoveComment(comment.Id);

        // Assert
        result.IsError.Should().BeFalse();
        wikiPage.Comments.Should().NotContain(comment);
    }

    [Fact]
    public void RemoveComment_WhenCommentDoesNotExist_ShouldReturnCommentNotFound()
    {
        // Arrange
        var wikiPage = WikiPageFactory.CreateWikiPage();
        var comment = new WikiPageComment(wikiPage.Id, Guid.NewGuid(), "Content");

        // Act
        var result = wikiPage.RemoveComment(comment.Id);

        // Assert
        result.FirstError.Should().Be(WikiPageErrors.CommentNotFound);
    }

    [Fact]
    public void InsertSubWikiPage_ShouldInsertSubWikiPage()
    {
        // Arrange
        var wikiPage = WikiPageFactory.CreateWikiPage();
        var subWikiPage = WikiPageFactory.CreateWikiPage();

        // Act
        wikiPage.InsertSubWikiPage(0, subWikiPage);

        // Assert
        wikiPage.SubWikiPages.Should().Contain(subWikiPage);
    }

    [Fact]
    public void RemoveParent_ShouldRemoveParent()
    {
        // Arrange
        var parentWikiPage = WikiPageFactory.CreateWikiPage();
        var wikiPage = WikiPageFactory.CreateWikiPage(parentWikiPageId: parentWikiPage.Id);

        // Act
        wikiPage.RemoveParent();

        // Assert
        wikiPage.ParentWikiPage.Should().BeNull();
    }
}