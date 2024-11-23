using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.AddMember;

public sealed record AddMemberCommand(
    Guid TeamId,
    Guid MemberId) : IRequest<ErrorOr<Success>>;