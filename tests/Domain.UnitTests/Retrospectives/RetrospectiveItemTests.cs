using Domain.Retrospectives;

namespace Domain.UnitTests.Retrospectives;

public class RetrospectiveItemTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange
        var content = "Great teamwork!";
        var retrospectiveColumnId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        // Act
        var item = new RetrospectiveItem(content, retrospectiveColumnId, authorId);

        // Assert
        item.Content.Should().Be(content);
        item.RetrospectiveColumnId.Should().Be(retrospectiveColumnId);
        item.AuthorId.Should().Be(authorId);
        item.Votes.Should().Be(0);
        item.Position.Should().Be(0);
    }

    [Fact]
    public void UpdatePosition_ShouldChangePosition()
    {
        // Arrange
        var item = new RetrospectiveItem(
            "Needs improvement",
            Guid.NewGuid(),
            Guid.NewGuid());
        var newPosition = 5;

        // Act
        item.UpdatePosition(newPosition);

        // Assert
        item.Position.Should().Be(newPosition);
    }
}
