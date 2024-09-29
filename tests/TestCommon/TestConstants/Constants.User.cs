using Application.Common.Security.Roles;

namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class User
    {
        public const string FirstName = "Service";
        public const string LastName = "Account";
        public const string Email = "svc_test_user@example.com";
        public const string Password = "2HdG!Ae!$Ib(j9Wp";
        public static readonly Guid Id = Guid.NewGuid();
        public static readonly List<string> Permissions = [];
        public static readonly List<string> Roles = [Role.Admin];
    }
}