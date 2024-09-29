using FluentValidation;

namespace Application.Boards.Commands.CreateBoard;

public class CreateBoardCommandValidator : AbstractValidator<CreateBoardCommand>
{
    public CreateBoardCommandValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}