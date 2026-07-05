namespace Domain.UnitTests.Users;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Act
        var user = UserFactory.Create();

        // Assert
        Assert.Equal(UserFactory.FirstName, user.FirstName);
        Assert.Equal(UserFactory.LastName, user.LastName);
        Assert.Equal(UserFactory.Email, user.Email);
        Assert.Equal($"{UserFactory.FirstName} {UserFactory.LastName}", user.FullName);
        Assert.Empty(user.IdentityId);
        Assert.Null(user.ImageFileId);
    }

    [Fact]
    public void SetIdentityId_ShouldUpdateIdentityId()
    {
        // Arrange
        var user = UserFactory.Create();
        var identityId = Guid.NewGuid().ToString();

        // Act
        user.SetIdentityId(identityId);

        // Assert
        Assert.Equal(identityId, user.IdentityId);
    }

    [Fact]
    public void SetImage_ShouldUpdateImageFileId()
    {
        // Arrange
        var user = UserFactory.Create();
        var imageFileId = Guid.NewGuid();

        // Act
        user.SetImage(imageFileId);

        // Assert
        Assert.Equal(imageFileId, user.ImageFileId);
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateFirstAndLastName()
    {
        // Arrange
        var user = UserFactory.Create();
        var newFirstName = "Jane";
        var newLastName = "Smith";

        // Act
        user.UpdateProfile(newFirstName, newLastName);

        // Assert
        Assert.Equal(newFirstName, user.FirstName);
        Assert.Equal(newLastName, user.LastName);
        Assert.Equal($"{newFirstName} {newLastName}", user.FullName);
    }
}
