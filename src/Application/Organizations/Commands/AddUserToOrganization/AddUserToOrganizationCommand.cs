using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.AddUserToOrganization;

public sealed record AddUserToOrganizationCommand(
    Guid OrganizationId,
    Guid UserId) : IRequest<ErrorOr<Success>>;