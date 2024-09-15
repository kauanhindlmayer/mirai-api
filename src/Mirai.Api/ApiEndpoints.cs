namespace Mirai.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "/api";

    public static class Authentication
    {
        public const string Register = $"{ApiBase}/register";
        public const string Login = $"{ApiBase}/login";
    }

    public static class Organizations
    {
        public const string Create = Base;
        public const string Get = $"{Base}/{{organizationId:guid}}";
        public const string List = Base;
        public const string Update = $"{Base}/{{organizationId:guid}}";
        public const string Delete = $"{Base}/{{organizationId:guid}}";
        private const string Base = $"{ApiBase}/organizations";
    }

    public static class Projects
    {
        public const string Create = Base;
        public const string Get = $"{Base}/{{projectId:guid}}";
        public const string List = Base;
        public const string Update = $"{Base}/{{projectId:guid}}";
        public const string Delete = $"{Base}/{{projectId:guid}}";
        public const string ListWorkItems = $"{Base}/{{projectId:guid}}/work-items";
        private const string Base = $"{ApiBase}/projects";
    }

    public static class WorkItems
    {
        public const string Create = Base;
        public const string Get = $"{Base}/{{workItemId:guid}}";
        public const string Update = $"{Base}/{{workItemId:guid}}";
        public const string Delete = $"{Base}/{{workItemId:guid}}";
        public const string AddComment = $"{Base}/{{workItemId:guid}}/comments";
        public const string Assign = $"{Base}/{{workItemId:guid}}/assign";
        private const string Base = $"{ApiBase}/work-items";
    }

    public static class WikiPages
    {
        public const string Create = Base;
        public const string Get = $"{Base}/{{wikiPageId:guid}}";
        public const string List = Base;
        public const string Update = $"{Base}/{{wikiPageId:guid}}";
        public const string Delete = $"{Base}/{{wikiPageId:guid}}";
        public const string AddComment = $"{Base}/{{wikiPageId:guid}}/comments";
        public const string Move = $"{Base}/{{wikiPageId:guid}}/move";
        private const string Base = $"{ApiBase}/wiki-pages";
    }
}