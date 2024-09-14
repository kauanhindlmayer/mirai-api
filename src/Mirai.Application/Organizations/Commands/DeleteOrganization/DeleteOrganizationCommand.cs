using ErrorOr;
using MediatR;

namespace Mirai.Application.Organizations.Commands.DeleteOrganization;

public record DeleteOrganizationCommand(Guid OrganizationId) : IRequest<ErrorOr<Success>>;