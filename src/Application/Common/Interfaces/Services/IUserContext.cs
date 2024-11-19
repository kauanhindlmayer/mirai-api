namespace Application.Common.Interfaces.Services;

public interface IUserContext
{
    Guid UserId { get; }
    string IdentityId { get; }
}
