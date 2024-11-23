using FluentValidation;

namespace Application.Retrospectives.Commands.DeleteItem;

internal sealed class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(x => x.RetrospectiveId)
            .NotEmpty();

        RuleFor(x => x.ColumnId)
            .NotEmpty();

        RuleFor(x => x.ItemId)
            .NotEmpty();
    }
}