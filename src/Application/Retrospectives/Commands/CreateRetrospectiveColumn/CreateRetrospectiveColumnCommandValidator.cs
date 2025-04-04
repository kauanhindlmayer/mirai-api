using FluentValidation;

namespace Application.Retrospectives.Commands.CreateRetrospectiveColumn;

public sealed class CreateRetrospectiveColumnCommandValidator : AbstractValidator<CreateRetrospectiveColumnCommand>
{
    public CreateRetrospectiveColumnCommandValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.RetrospectiveId)
            .NotEmpty();
    }
}