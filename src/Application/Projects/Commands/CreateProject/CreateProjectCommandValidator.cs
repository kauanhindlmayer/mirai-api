using FluentValidation;

namespace Application.Projects.Commands.CreateProject;

internal sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.OrganizationId)
            .NotEmpty();
    }
}