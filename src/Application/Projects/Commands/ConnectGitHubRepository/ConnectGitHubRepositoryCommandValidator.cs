using FluentValidation;

namespace Application.Projects.Commands.ConnectGitHubRepository;

internal sealed class ConnectGitHubRepositoryCommandValidator : AbstractValidator<ConnectGitHubRepositoryCommand>
{
    public ConnectGitHubRepositoryCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.InstallationId)
            .GreaterThan(0);

        RuleFor(x => x.RepositoryId)
            .GreaterThan(0);

        RuleFor(x => x.RepositoryOwner)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(255);
    }
}
