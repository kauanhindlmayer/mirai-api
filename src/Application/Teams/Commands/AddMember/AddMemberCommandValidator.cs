using FluentValidation;

namespace Application.Teams.Commands.AddMember;

public sealed class AddMemberCommandValidator : AbstractValidator<AddMemberCommand>
{
    public AddMemberCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.MemberId)
            .NotEmpty();
    }
}