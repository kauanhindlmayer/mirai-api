using Domain.Notifications;

namespace Domain.UnitTests.Notifications;

public class MentionParserTests
{
    [Fact]
    public void ParseMentionedUserIds_WithSingleMention_ShouldReturnUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var html = $"""Thanks <span data-type="mention" data-id="{userId}" data-label="Jane Smith">@Jane Smith</span>""";

        // Act
        var userIds = MentionParser.ParseMentionedUserIds(html);

        // Assert
        userIds.Should().BeEquivalentTo([userId]);
    }

    [Fact]
    public void ParseMentionedUserIds_WithMultipleMentions_ShouldReturnAllDistinctUserIds()
    {
        // Arrange
        var firstUserId = Guid.NewGuid();
        var secondUserId = Guid.NewGuid();
        var html =
            $"""<span data-type="mention" data-id="{firstUserId}">@A</span> and """ +
            $"""<span data-type="mention" data-id="{secondUserId}">@B</span> and """ +
            $"""<span data-type="mention" data-id="{firstUserId}">@A</span> again""";

        // Act
        var userIds = MentionParser.ParseMentionedUserIds(html);

        // Assert
        userIds.Should().BeEquivalentTo([firstUserId, secondUserId]);
    }

    [Fact]
    public void ParseMentionedUserIds_WithNoMentions_ShouldReturnEmptySet()
    {
        // Act
        var userIds = MentionParser.ParseMentionedUserIds("Just plain text, no mentions here.");

        // Assert
        userIds.Should().BeEmpty();
    }

    [Fact]
    public void ParseMentionedUserIds_WithNonMentionSpan_ShouldNotMatch()
    {
        // Act
        var userIds = MentionParser.ParseMentionedUserIds(
            """<span class="highlight" data-id="not-a-mention">Text</span>""");

        // Assert
        userIds.Should().BeEmpty();
    }

    [Fact]
    public void ParseMentionedUserIds_WithNonGuidDataId_ShouldIgnoreIt()
    {
        // Act
        var userIds = MentionParser.ParseMentionedUserIds(
            """<span data-type="mention" data-id="not-a-guid">@Someone</span>""");

        // Assert
        userIds.Should().BeEmpty();
    }
}
