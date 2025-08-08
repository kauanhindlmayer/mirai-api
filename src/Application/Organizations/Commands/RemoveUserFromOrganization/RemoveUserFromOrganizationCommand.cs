using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.RemoveUserFromOrganization;

public sealed record RemoveUserFromOrganizationCommand(
    Guid OrganizationId,
    Guid UserId) : IRequest<ErrorOr<Success>>;