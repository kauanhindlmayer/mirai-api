using FluentValidation;

namespace Application.Projects.Commands.RemoveUserFromProject;

internal sealed class RemoveUserFromProjectCommandValidator : AbstractValidator<RemoveUserFromProjectCommand>
{
    public RemoveUserFromProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}