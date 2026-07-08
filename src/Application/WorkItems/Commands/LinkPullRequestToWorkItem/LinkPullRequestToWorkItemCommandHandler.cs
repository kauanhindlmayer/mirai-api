using Application.Abstractions.GitHub;
using Domain.Projects;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.LinkPullRequestToWorkItem;

internal sealed class LinkPullRequestToWorkItemCommandHandler
    : IRequestHandler<LinkPullRequestToWorkItemCommand, ErrorOr<Guid>>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IGitHubPullRequestService _pullRequestService;

    public LinkPullRequestToWorkItemCommandHandler(
        IWorkItemRepository workItemRepository,
        IProjectRepository projectRepository,
        IGitHubPullRequestService pullRequestService)
    {
        _workItemRepository = workItemRepository;
        _projectRepository = projectRepository;
        _pullRequestService = pullRequestService;
    }

    public async Task<ErrorOr<Guid>> Handle(
        LinkPullRequestToWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdWithPullRequestLinksAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var project = await _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            workItem.ProjectId,
            cancellationToken);

        var connection = project?.GitHubRepositoryConnection;
        if (connection is null)
        {
            return ProjectErrors.NoGitHubRepositoryConnected;
        }

        var pullRequest = await _pullRequestService.GetPullRequestAsync(
            connection.InstallationId,
            connection.RepositoryOwner,
            connection.RepositoryName,
            command.PullRequestNumber,
            cancellationToken);

        if (pullRequest is null)
        {
            return WorkItemErrors.GitHubPullRequestNotFound;
        }

        var link = new WorkItemPullRequestLink(
            workItem.Id,
            pullRequest.Id,
            pullRequest.Number,
            pullRequest.Title,
            pullRequest.HtmlUrl,
            ResolveState(pullRequest),
            pullRequest.AuthorLogin,
            PullRequestLinkSource.Manual);

        var result = workItem.AddPullRequestLink(link);
        if (result.IsError)
        {
            return result.Errors;
        }

        _workItemRepository.Update(workItem);

        return link.Id;
    }

    private static PullRequestLinkState ResolveState(GitHubPullRequestSummary pullRequest)
    {
        if (pullRequest.IsMerged)
        {
            return PullRequestLinkState.Merged;
        }

        return pullRequest.IsOpen ? PullRequestLinkState.Open : PullRequestLinkState.Closed;
    }
}
