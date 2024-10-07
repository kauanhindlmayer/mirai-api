using Application.Common.Interfaces;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.AddMember;

public class AddMemberCommandHandler(
    ITeamsRepository _teamsRepository,
    IUsersRepository _usersRepository)
    : IRequestHandler<AddMemberCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        AddMemberCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamsRepository.GetByIdAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var member = await _usersRepository.GetByIdAsync(
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

        _teamsRepository.Update(team);

        return Result.Success;
    }
}