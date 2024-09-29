using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.DeleteOrganization;

public record DeleteOrganizationCommand(Guid OrganizationId)
    : IRequest<ErrorOr<Success>>;