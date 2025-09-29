using Application.Abstractions.Sorting;
using Application.Organizations.Queries.GetOrganizationUsers;
using Domain.Users;

namespace Application.Abstractions.Mappings;

internal static class OrganizationUserMappings
{
    public static readonly SortMappingDefinition<OrganizationUserResponse, User> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(OrganizationUserResponse.FullName), nameof(User.FirstName)),
            new SortMapping(nameof(OrganizationUserResponse.Email), nameof(User.Email)),
            new SortMapping(nameof(OrganizationUserResponse.LastActiveAtUtc), nameof(User.LastActiveAtUtc)),
        ],
    };
}