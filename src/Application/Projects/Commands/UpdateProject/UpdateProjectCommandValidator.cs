using FluentValidation;

namespace Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}