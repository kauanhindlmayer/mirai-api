namespace Application.Common.Interfaces;

public interface IUserContext
{
    Guid UserId { get; }
    string IdentityId { get; }
}
