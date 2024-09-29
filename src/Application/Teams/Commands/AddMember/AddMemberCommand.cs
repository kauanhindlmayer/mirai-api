using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.AddMember;

public record AddMemberCommand(Guid TeamId, Guid MemberId)
    : IRequest<ErrorOr<Success>>;