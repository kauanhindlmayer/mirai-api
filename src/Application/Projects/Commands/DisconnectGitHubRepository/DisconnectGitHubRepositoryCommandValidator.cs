using FluentValidation;

namespace Application.Projects.Commands.DisconnectGitHubRepository;

internal sealed class DisconnectGitHubRepositoryCommandValidator : AbstractValidator<DisconnectGitHubRepositoryCommand>
{
    public DisconnectGitHubRepositoryCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}
