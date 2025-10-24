using Application.Abstractions.Sorting;
using Application.Projects.Queries.GetProjectUsers;
using Domain.Users;

namespace Application.Abstractions.Mappings;

internal static class ProjectUserMappings
{
    public static readonly SortMappingDefinition<ProjectUserResponse, User> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(ProjectUserResponse.FullName), nameof(User.FirstName)),
            new SortMapping(nameof(ProjectUserResponse.Email), nameof(User.Email)),
        ],
    };
}