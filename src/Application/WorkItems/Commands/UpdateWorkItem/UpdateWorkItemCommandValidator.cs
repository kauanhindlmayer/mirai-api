using Application.Tags.Commands.Common;
using Domain.WorkItems.ValueObjects;
using FluentValidation;

namespace Application.WorkItems.Commands.UpdateWorkItem;

internal sealed class UpdateWorkItemCommandValidator : AbstractValidator<UpdateWorkItemCommand>
{
    public UpdateWorkItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100)
            .WithMessage("Title must be at most 100 characters.")
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .When(x => x.Title is not null)
            .WithMessage("Title cannot be empty or whitespace.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null)
            .WithMessage("Description must be at most 1000 characters.");

        RuleFor(x => x.AcceptanceCriteria)
            .MaximumLength(1000)
            .When(x => x.AcceptanceCriteria is not null)
            .WithMessage("Acceptance criteria must be at most 1000 characters.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("Invalid work item type.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Invalid work item status.");

        RuleFor(x => x.AssigneeId)
            .NotEmpty()
            .When(x => x.AssigneeId.HasValue)
            .WithMessage("Assignee ID must be a valid GUID.");

        RuleFor(x => x.AssignedTeamId)
            .NotEmpty()
            .When(x => x.AssignedTeamId.HasValue)
            .WithMessage("Assigned team ID must be a valid GUID.");

        RuleFor(x => x.SprintId)
            .NotEmpty()
            .When(x => x.SprintId.HasValue)
            .WithMessage("Sprint ID must be a valid GUID.");

        RuleFor(x => x.ParentWorkItemId)
            .NotEmpty()
            .When(x => x.ParentWorkItemId.HasValue)
            .WithMessage("Parent work item ID must be a valid GUID.");

        RuleFor(x => x.Planning)
            .SetValidator(new PlanningValidator()!)
            .When(x => x.Planning is not null);

        RuleFor(x => x.Classification)
            .SetValidator(new ClassificationValidator()!)
            .When(x => x.Classification is not null);
    }
}

internal sealed class PlanningValidator : AbstractValidator<Planning>
{
    public PlanningValidator()
    {
        RuleFor(x => x.StoryPoints)
            .GreaterThanOrEqualTo(0)
            .When(x => x.StoryPoints.HasValue)
            .WithMessage("Story points must be zero or positive.");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 5)
            .When(x => x.Priority.HasValue)
            .WithMessage("Priority must be between 0 and 5.");
    }
}

internal sealed class ClassificationValidator : AbstractValidator<Classification>
{
    public ClassificationValidator()
    {
        RuleFor(x => x.ValueArea)
            .IsInEnum()
            .WithMessage("Invalid value area.");
    }
}
