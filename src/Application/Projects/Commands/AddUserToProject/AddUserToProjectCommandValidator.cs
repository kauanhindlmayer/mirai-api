using FluentValidation;

namespace Application.Projects.Commands.AddUserToProject;

internal sealed class AddUserToProjectCommandValidator : AbstractValidator<AddUserToProjectCommand>
{
    public AddUserToProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}