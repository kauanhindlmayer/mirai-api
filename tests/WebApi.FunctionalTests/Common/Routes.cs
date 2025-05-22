namespace WebApi.FunctionalTests.Common;

public static class Routes
{
    public static class Organizations
    {
        public const string Base = "/api/organizations";
        public const string List = Base;
        public const string Create = Base;
        public static string Get(Guid organizationId) => $"{Base}/{organizationId}";
        public static string Update(Guid organizationId) => $"{Base}/{organizationId}";
        public static string Delete(Guid organizationId) => $"{Base}/{organizationId}";
    }

    public static class Projects
    {
        public const string Base = "/api/organizations";
        public static string Create(Guid organizationId) => $"{Base}/{organizationId}/projects";
        public static string Get(Guid projectId) => $"/api/projects/{projectId}";
    }

    public static class Users
    {
        public const string Base = "/api/users";
        public const string Register = Base;
        public const string Login = $"{Base}/login";
        public const string GetLoggedInUser = $"{Base}/me";
        public const string UpdateProfile = $"{Base}/profile";
    }
}