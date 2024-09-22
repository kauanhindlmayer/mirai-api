using ErrorOr;
using MediatR;

namespace Mirai.Application.Teams.Commands.AddMember;

public record AddMemberCommand(Guid TeamId, Guid MemberId)
    : IRequest<ErrorOr<Success>>;