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
}