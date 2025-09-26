using Application.Abstractions;
using Application.Abstractions.Authentication;
using Domain.Projects;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.CreateWikiPage;

internal sealed class CreateWikiPageCommandHandler
    : IRequestHandler<CreateWikiPageCommand, ErrorOr<Guid>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;
    private readonly IHtmlSanitizerService _htmlSanitizerService;

    public CreateWikiPageCommandHandler(
        IProjectRepository projectRepository,
        IUserContext userContext,
        IHtmlSanitizerService htmlSanitizerService)
    {
        _projectRepository = projectRepository;
        _userContext = userContext;
        _htmlSanitizerService = htmlSanitizerService;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithWikiPagesAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var sanitizedContent = _htmlSanitizerService.Sanitize(command.Content);

        var wikiPage = new WikiPage(
            command.ProjectId,
            command.Title,
            sanitizedContent,
            _userContext.UserId,
            command.ParentWikiPageId);

        var result = project.AddWikiPage(wikiPage);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectRepository.Update(project);

        return wikiPage.Id;
    }
}