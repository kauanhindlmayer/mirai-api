using ErrorOr;

namespace Domain.Organizations;

public static class OrganizationErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Organization.NotFound",
        "Organization not found.");

    public static readonly Error AlreadyExists = Error.Validation(
        "Organization.AlreadyExists",
        "An organization with the same name already exists.");

    public static readonly Error UserAlreadyExists = Error.Conflict(
      "Organization.UserAlreadyExists",
      "User already exists in this organization.");

    public static readonly Error UserHasProjects = Error.Conflict(
        "Organization.UserHasProjects",
        "Cannot remove user who has projects in this organization.");
}