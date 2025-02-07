using FluentValidation;

namespace Application.Retrospectives.Commands.DeleteRetrospectiveItem;

public sealed class DeleteRetrospectiveItemCommandValidator : AbstractValidator<DeleteRetrospectiveItemCommand>
{
    public DeleteRetrospectiveItemCommandValidator()
    {
        RuleFor(x => x.RetrospectiveId)
            .NotEmpty();

        RuleFor(x => x.ColumnId)
            .NotEmpty();

        RuleFor(x => x.ItemId)
            .NotEmpty();
    }
}