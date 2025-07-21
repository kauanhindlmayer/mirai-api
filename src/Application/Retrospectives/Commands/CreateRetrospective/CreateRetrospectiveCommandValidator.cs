using FluentValidation;

namespace Application.Retrospectives.Commands.CreateRetrospective;

internal sealed class CreateRetrospectiveCommandValidator : AbstractValidator<CreateRetrospectiveCommand>
{
    public CreateRetrospectiveCommandValidator()
    {
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

        RuleFor(x => x.TeamId)
            .NotEmpty();
    }
}