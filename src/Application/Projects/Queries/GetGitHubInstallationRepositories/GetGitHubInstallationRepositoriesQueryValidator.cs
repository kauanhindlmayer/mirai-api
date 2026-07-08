using FluentValidation;

namespace Application.Projects.Queries.GetGitHubInstallationRepositories;

internal sealed class GetGitHubInstallationRepositoriesQueryValidator
    : AbstractValidator<GetGitHubInstallationRepositoriesQuery>
{
    public GetGitHubInstallationRepositoriesQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.InstallationId)
            .GreaterThan(0);

        RuleFor(x => x.State)
            .NotEmpty();
    }
}
