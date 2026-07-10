using Domain.Organizations;
using Domain.Projects;
using Domain.Shared;
using Domain.Teams;
using Domain.WorkItems;

namespace Domain.Users;

public sealed class User : Entity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; private set; } = null!;

    /// <summary>
    /// The unique identifier for the user in the identity provider.
    /// </summary>
    public string IdentityId { get; private set; } = string.Empty;
    public Guid? ImageFileId { get; private set; }
    public DateTime? LastActiveAtUtc { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiresAtUtc { get; private set; }
    public ICollection<WorkItem> WorkItems { get; private set; } = [];
    public ICollection<OrganizationMember> OrganizationMemberships { get; private set; } = [];
    public ICollection<ProjectMember> ProjectMemberships { get; private set; } = [];
    public ICollection<TeamMember> TeamMemberships { get; private set; } = [];

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

    public void SetImage(Guid imageFileId)
    {
        ImageFileId = imageFileId;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public void UpdateLastActive()
    {
        LastActiveAtUtc = DateTime.UtcNow;
    }

    public void SetPasswordResetToken(string token, DateTime expiresAtUtc)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiresAtUtc = expiresAtUtc;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiresAtUtc = null;
    }
}