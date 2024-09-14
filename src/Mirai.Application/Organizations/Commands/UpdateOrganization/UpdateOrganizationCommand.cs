using ErrorOr;
using MediatR;
using Mirai.Domain.Organizations;

namespace Mirai.Application.Organizations.Commands.UpdateOrganization;

public record UpdateOrganizationCommand(Guid Id, string Name, string? Description)
    : IRequest<ErrorOr<Organization>>;