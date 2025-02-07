using FluentValidation;

namespace Application.Retrospectives.Commands.CreateRetrospectiveItem;

public sealed class CreateRetrospectiveItemCommandValidator : AbstractValidator<CreateRetrospectiveItemCommand>
{
    public CreateRetrospectiveItemCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.ColumnId)
            .NotEmpty();
    }
}