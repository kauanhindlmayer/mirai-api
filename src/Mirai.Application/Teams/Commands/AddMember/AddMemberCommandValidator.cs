using FluentValidation;

namespace Mirai.Application.Teams.Commands.AddMember;

public class AddMemberCommandValidator : AbstractValidator<AddMemberCommand>
{
    public AddMemberCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.MemberId)
            .NotEmpty();
    }
}