using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.UpdateOrganization;

public record UpdateOrganizationCommand(Guid Id, string Name, string? Description)
    : IRequest<ErrorOr<Organization>>;