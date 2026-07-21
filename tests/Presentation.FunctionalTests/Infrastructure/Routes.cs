namespace Presentation.FunctionalTests.Infrastructure;

public static class Routes
{
    public static class Organizations
    {
        public const string List = Base;
        public const string Create = Base;
        public static string Get(Guid organizationId) => $"{Base}/{organizationId}";
        public static string Update(Guid organizationId) => $"{Base}/{organizationId}";
        public static string Delete(Guid organizationId) => $"{Base}/{organizationId}";
        public static string AddUser(Guid organizationId) => $"{Base}/{organizationId}/users";
        public static string GetUserProfile(Guid organizationId, Guid userId) =>
            $"{Base}/{organizationId}/users/{userId}/profile";
        private const string Base = "/api/organizations";
    }

    public static class Projects
    {
        public static string Create(Guid organizationId) => $"{Base}/{organizationId}/projects";
        public static string Get(Guid projectId) => $"/api/projects/{projectId}";
        public static string AddUser(Guid organizationId, Guid projectId) =>
            $"{Base}/{organizationId}/projects/{projectId}/users";
        private const string Base = "/api/organizations";
    }

    public static class Teams
    {
        public static string Create(Guid projectId) => $"/api/projects/{projectId}/teams";
        public static string AddMember(Guid projectId, Guid teamId) =>
            $"/api/projects/{projectId}/teams/{teamId}/members";
    }

    public static class Users
    {
        public const string Register = $"{Base}/register";
        public const string Login = $"{Base}/login";
        public const string LoginWithGitHub = $"{Base}/login/github";
        public const string GetLoggedInUser = $"{Base}/me";
        public const string UpdateProfile = $"{Base}/profile";
        public const string ForgotPassword = $"{Base}/forgot-password";
        public const string ResetPassword = $"{Base}/reset-password";
        private const string Base = "/api/users";
    }
}