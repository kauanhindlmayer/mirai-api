namespace WebApi.FunctionalTests.Common;

public static class Routes
{
    public static class Organizations
    {
        public const string Base = "/api/organizations";
        public const string List = Base;
        public const string Create = Base;
        public static string Get(Guid id) => $"{Base}/{id}";
        public static string Update(Guid id) => $"{Base}/{id}";
        public static string Delete(Guid id) => $"{Base}/{id}";
    }
}