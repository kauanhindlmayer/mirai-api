using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.DeleteOrganization;

public sealed record DeleteOrganizationCommand(Guid OrganizationId)
    : IRequest<ErrorOr<Success>>;