using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Users.Queries.GetUserProfile;

public sealed record GetUserProfileQuery(
    Guid OrganizationId,
    Guid UserId) : IAuthorizationRequest<ErrorOr<UserProfileResponse>>
{
    public Permission RequiredPermission => Permission.OrganizationView;
    public ResourceType ResourceType => ResourceType.Organization;
    public Guid ResourceId => OrganizationId;
}
