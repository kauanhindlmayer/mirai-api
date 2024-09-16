using ErrorOr;

namespace Mirai.Domain.Organizations;

public static class OrganizationErrors
{
    public static readonly Error OrganizationNotFound = Error.NotFound(
        "Organization.OrganizationNotFound",
        "Organization not found.");

    public static readonly Error OrganizationWithSameNameAlreadyExists = Error.Validation(
        "Organization.OrganizationWithSameNameAlreadyExists",
        "An organization with the same name already exists.");
}