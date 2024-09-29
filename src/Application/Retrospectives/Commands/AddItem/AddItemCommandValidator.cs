using FluentValidation;

namespace Application.Retrospectives.Commands.AddItem;

public class AddItemCommandValidator : AbstractValidator<AddItemCommand>
{
    public AddItemCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.RetrospectiveColumnId)
            .NotEmpty();
    }
}