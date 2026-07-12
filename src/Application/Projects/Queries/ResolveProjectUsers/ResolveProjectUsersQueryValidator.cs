using FluentValidation;

namespace Application.Projects.Queries.ResolveProjectUsers;

internal sealed class ResolveProjectUsersQueryValidator : AbstractValidator<ResolveProjectUsersQuery>
{
    private const int MaxUserIds = 100;

    public ResolveProjectUsersQueryValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserIds)
            .NotEmpty()
            .Must(userIds => userIds.Count <= MaxUserIds)
            .WithMessage($"No more than {MaxUserIds} user ids can be resolved at once.");
    }
}
