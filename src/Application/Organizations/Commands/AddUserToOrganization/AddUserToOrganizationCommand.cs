using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.AddUserToOrganization;

public sealed record AddUserToOrganizationCommand(
    Guid OrganizationId,
    string Email) : IRequest<ErrorOr<Success>>;