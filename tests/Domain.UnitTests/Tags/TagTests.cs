namespace Domain.UnitTests.Tags;

public class TagTests
{
    [Fact]
    public void CreateTag_ShouldSetProperties()
    {
        // Act
        var tag = TagFactory.CreateTag();

        // Assert
        tag.ProjectId.Should().NotBeEmpty();
        tag.Name.Should().Be(TagFactory.Name);
        tag.Description.Should().Be(TagFactory.Description);
        tag.Color.Should().Be(TagFactory.Color);
    }

    [Fact]
    public void Update_ShouldUpdateProperties()
    {
        // Arrange
        var tag = TagFactory.CreateTag();
        var name = "Updated Name";
        var description = "Updated Description";
        var color = "#FFFFFF";

        // Act
        tag.Update(name, description, color);

        // Assert
        tag.Name.Should().Be(name);
        tag.Description.Should().Be(description);
        tag.Color.Should().Be(color);
    }
}