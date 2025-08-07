using Domain.Teams;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.AddMember;

internal sealed class AddMemberCommandHandler : IRequestHandler<AddMemberCommand, ErrorOr<Success>>
{
    private readonly ITeamsRepository _teamsRepository;
    private readonly IUsersRepository _usersRepository;

    public AddMemberCommandHandler(
        ITeamsRepository teamsRepository,
        IUsersRepository usersRepository)
    {
        _teamsRepository = teamsRepository;
        _usersRepository = usersRepository;
    }

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