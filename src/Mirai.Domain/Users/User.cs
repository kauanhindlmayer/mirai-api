using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Common;
using Mirai.Domain.WorkItems;

namespace Mirai.Domain.Users;

public class User : Entity
{
    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public ICollection<WorkItem> WorkItems { get; private set; } = [];

    public User(string firstName, string lastName, string email, string passwordHash)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
    }

    private User()
    {
    }

    public bool IsCorrectPasswordHash(string password, IPasswordHasher passwordHasher)
    {
        return passwordHasher.IsCorrectPassword(password, PasswordHash);
    }
}