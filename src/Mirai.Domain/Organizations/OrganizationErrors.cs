using ErrorOr;

namespace Mirai.Domain.Organizations;

public static class OrganizationErrors
{
    public static readonly Error OrganizationWithSameNameAlreadyExists = Error.Validation(
        "Organization.OrganizationWithSameNameAlreadyExists",
        "An organization with the same name already exists.");

    public static readonly Error ProjectWithSameNameAlreadyExists = Error.Validation(
        "Project.ProjectWithSameNameAlreadyExists",
        "A project with the same name already exists in the organization.");
}