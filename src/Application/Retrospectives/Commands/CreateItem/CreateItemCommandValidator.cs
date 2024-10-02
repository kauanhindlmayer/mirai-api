using FluentValidation;

namespace Application.Retrospectives.Commands.CreateItem;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.RetrospectiveColumnId)
            .NotEmpty();
    }
}