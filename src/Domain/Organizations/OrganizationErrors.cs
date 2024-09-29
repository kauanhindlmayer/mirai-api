using ErrorOr;

namespace Domain.Organizations;

public static class OrganizationErrors
{
    public static readonly Error OrganizationNotFound = Error.NotFound(
        "Organization.OrganizationNotFound",
        "Organization not found.");

    public static readonly Error OrganizationWithSameNameAlreadyExists = Error.Validation(
        "Organization.OrganizationWithSameNameAlreadyExists",
        "An organization with the same name already exists.");

    public static readonly Error UserAlreadyMember = Error.Validation(
        "Organization.UserAlreadyMember",
        "The user is already a member of the organization.");

    public static readonly Error UserNotMember = Error.Validation(
        "Organization.UserNotMember",
        "The user is not a member of the organization.");
}