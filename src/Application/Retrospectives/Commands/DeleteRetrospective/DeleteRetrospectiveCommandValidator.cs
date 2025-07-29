using FluentValidation;

namespace Application.Retrospectives.Commands.DeleteRetrospective;

internal sealed class DeleteRetrospectiveCommandValidator
    : AbstractValidator<DeleteRetrospectiveCommand>
{
    public DeleteRetrospectiveCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.RetrospectiveId)
            .NotEmpty();
    }
}