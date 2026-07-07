using FluentValidation;

namespace Application.Projects.Queries.GetGitHubInstallUrl;

internal sealed class GetGitHubInstallUrlQueryValidator : AbstractValidator<GetGitHubInstallUrlQuery>
{
    public GetGitHubInstallUrlQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}
