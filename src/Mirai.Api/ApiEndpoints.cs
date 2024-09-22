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
        public const string ListProjects = $"{Base}/{{organizationId:guid}}/projects";
        public const string ListMembers = $"{Base}/{{organizationId:guid}}/members";
        private const string Base = $"{ApiBase}/organizations";
    }

    public static class Projects
    {
        public const string Create = Base;
        public const string Get = $"{Base}/{{projectId:guid}}";
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

    public static class Retrospectives
    {
        public const string Create = Base;
        public const string Get = $"{Base}/{{retrospectiveId:guid}}";
        public const string Update = $"{Base}/{{retrospectiveId:guid}}";
        public const string Delete = $"{Base}/{{retrospectiveId:guid}}";
        public const string AddColumn = $"{Base}/{{retrospectiveId:guid}}/columns";
        public const string AddItem = $"{Base}/{{retrospectiveId:guid}}/columns/{{columnId:guid}}/items";
        private const string Base = $"{ApiBase}/retrospectives";
    }

    public static class Teams
    {
        public const string Create = Base;
        public const string Get = $"{Base}/{{teamId:guid}}";
        public const string Update = $"{Base}/{{teamId:guid}}";
        public const string Delete = $"{Base}/{{teamId:guid}}";
        public const string AddMember = $"{Base}/{{teamId:guid}}/members";
        public const string RemoveMember = $"{Base}/{{teamId:guid}}/members";
        private const string Base = $"{ApiBase}/teams";
    }
}