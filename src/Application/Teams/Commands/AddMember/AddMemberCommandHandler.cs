using Application.Common.Interfaces.Persistence;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.AddMember;

internal sealed class AddMemberCommandHandler(
    ITeamsRepository teamsRepository,
    IUsersRepository usersRepository)
    : IRequestHandler<AddMemberCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        AddMemberCommand command,
        CancellationToken cancellationToken)
    {
        var team = await teamsRepository.GetByIdAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var member = await usersRepository.GetByIdAsync(
            command.MemberId,
            cancellationToken);

        if (member is null)
        {
            return TeamErrors.MemberNotFound;
        }

        var result = team.AddMember(member);
        if (result.IsError)
        {
            return result.Errors;
        }

        teamsRepository.Update(team);

        return Result.Success;
    }
}