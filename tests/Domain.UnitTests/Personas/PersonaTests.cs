namespace Domain.UnitTests.Personas;

public class PersonaTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Act
        var persona = PersonaFactory.Create();

        // Assert
        persona.ProjectId.Should().Be(PersonaFactory.ProjectId);
        persona.Name.Should().Be(PersonaFactory.Name);
        persona.Category.Should().Be(PersonaFactory.Category);
        persona.Description.Should().Be(PersonaFactory.Description);
        persona.ImageFileId.Should().BeNull();
    }

    [Fact]
    public void SetImage_ShouldUpdateImageFileId()
    {
        // Arrange
        var persona = PersonaFactory.Create();
        var imageFileId = Guid.NewGuid();

        // Act
        persona.SetImage(imageFileId);

        // Assert
        persona.ImageFileId.Should().Be(imageFileId);
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateNameAndDescription()
    {
        // Arrange
        var persona = PersonaFactory.Create();
        var newName = "Updated Name";
        var newDescription = "Updated description";

        // Act
        persona.Update(newName, newDescription);

        // Assert
        persona.Name.Should().Be(newName);
        persona.Description.Should().Be(newDescription);
        persona.ProjectId.Should().Be(PersonaFactory.ProjectId);
    }

    [Fact]
    public void Update_WithNullDescription_ShouldClearDescription()
    {
        // Arrange
        var persona = PersonaFactory.Create();
        var newName = "Updated Name";

        // Act
        persona.Update(newName, null);

        // Assert
        persona.Name.Should().Be(newName);
        persona.Description.Should().BeNull();
    }
}