using FluentValidation;

namespace Application.Retrospectives.Commands.RemoveItem;

public class RemoveItemCommandValidator : AbstractValidator<RemoveItemCommand>
{
    public RemoveItemCommandValidator()
    {
        RuleFor(x => x.RetrospectiveId)
            .NotEmpty();

        RuleFor(x => x.ColumnId)
            .NotEmpty();

        RuleFor(x => x.ItemId)
            .NotEmpty();
    }
}