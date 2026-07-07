using FluentValidation;

namespace Application.Projects.Queries.SearchGitHubPullRequests;

internal sealed class SearchGitHubPullRequestsQueryValidator : AbstractValidator<SearchGitHubPullRequestsQuery>
{
    public SearchGitHubPullRequestsQueryValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}
