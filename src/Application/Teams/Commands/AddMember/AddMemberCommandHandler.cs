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
        AddMemberCommand request,
        CancellationToken cancellationToken)
    {
        var team = await _teamsRepository.GetByIdAsync(
            request.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.TeamNotFound;
        }

        var member = await _usersRepository.GetByIdAsync(
            request.MemberId,
            cancellationToken);

        if (member is null)
        {
            return TeamErrors.TeamMemberNotFound;
        }

        var addMemberResult = team.AddMember(member);
        if (addMemberResult.IsError)
        {
            return addMemberResult.Errors;
        }

        _teamsRepository.Update(team);

        return Result.Success;
    }
}