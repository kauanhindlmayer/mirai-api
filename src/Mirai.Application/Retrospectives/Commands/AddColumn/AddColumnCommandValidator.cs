using FluentValidation;

namespace Mirai.Application.Retrospectives.Commands.AddColumn;

public class AddColumnCommandValidator : AbstractValidator<AddColumnCommand>
{
    public AddColumnCommandValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.RetrospectiveId)
            .NotEmpty();
    }
}