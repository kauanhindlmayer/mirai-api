using FluentValidation;

namespace Application.Users.Commands.LoginWithGitHub;

internal sealed class LoginWithGitHubCommandValidator : AbstractValidator<LoginWithGitHubCommand>
{
    public LoginWithGitHubCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.RedirectUri)
            .NotEmpty();
    }
}
