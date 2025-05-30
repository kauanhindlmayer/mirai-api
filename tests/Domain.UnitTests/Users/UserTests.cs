using Domain.UnitTests.Common;

namespace Domain.UnitTests.Users;

public class UsersTests : BaseTest
{
    [Fact]
    public void CreateUser_ShouldSetProperties()
    {
        // Act
        var user = UserFactory.CreateUser();

        // Assert
        user.Email.Should().Be(UserFactory.Email);
        user.FirstName.Should().Be(UserFactory.FirstName);
        user.LastName.Should().Be(UserFactory.LastName);
        user.FullName.Should().Be(UserFactory.FullName);
    }

    [Fact]
    public void SetIdentityId_ShouldSetIdentityId()
    {
        // Arrange
        var user = UserFactory.CreateUser();

        // Act
        user.SetIdentityId(UserFactory.IdentityId);

        // Assert
        user.IdentityId.Should().Be(UserFactory.IdentityId);
    }

    [Fact]
    public void SetImageUrl_ShouldSetImageUrl()
    {
        // Arrange
        var user = UserFactory.CreateUser();

        // Act
        user.SetImage(UserFactory.ImageUrl, UserFactory.ImageFileId);

        // Assert
        user.ImageUrl.Should().Be(UserFactory.ImageUrl);
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateProfile()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var firstName = "Jane";
        var lastName = "Smith";

        // Act
        user.UpdateProfile(firstName, lastName);

        // Assert
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.FullName.Should().Be($"{firstName} {lastName}");
    }
}