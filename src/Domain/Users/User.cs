using Domain.Common;
using Domain.Organizations;
using Domain.WorkItems;

namespace Domain.Users;

public sealed class User : Entity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; private set; } = null!;
    public string IdentityId { get; private set; } = string.Empty;
    public ICollection<WorkItem> WorkItems { get; private set; } = [];
    public ICollection<Organization> Organizations { get; private set; } = [];

    public User(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    private User()
    {
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}