using FluentValidation;

namespace Mirai.Application.Retrospectives.Commands.CreateRetrospective;

public class CreateRetrospectiveCommandValidator : AbstractValidator<CreateRetrospectiveCommand>
{
    public CreateRetrospectiveCommandValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.TeamId)
            .NotEmpty();
    }
}