using Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure;

public sealed class LinkService
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LinkService(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public LinkResponse Create(
        string actionName,
        string rel,
        string method,
        object? routeValues = null,
        string? controllerName = null)
    {
        string? href = _linkGenerator.GetUriByAction(
            _httpContextAccessor.HttpContext!,
            actionName,
            controllerName,
            routeValues);

        if (href is null)
        {
            throw new Exception($"Failed to generate link for action '{actionName}'");
        }

        return new LinkResponse
        {
            Href = href,
            Rel = rel,
            Method = method,
        };
    }
}
