using FluentValidation;

namespace Application.Retrospectives.Commands.UpdateRetrospective;

internal sealed class UpdateRetrospectiveCommandValidator : AbstractValidator<UpdateRetrospectiveCommand>
{
    public UpdateRetrospectiveCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.RetrospectiveId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.MaxVotesPerUser)
            .GreaterThanOrEqualTo(3)
            .LessThanOrEqualTo(12)
            .When(x => x.MaxVotesPerUser.HasValue);

        RuleFor(x => x.Template)
            .IsInEnum()
            .When(x => x.Template.HasValue);
    }
}