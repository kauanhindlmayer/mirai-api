using ErrorOr;
using MediatR;
using Mirai.Domain.Organizations;

namespace Mirai.Application.Organizations.Commands.CreateOrganization;

public record CreateOrganizationCommand(string Name, string? Description)
    : IRequest<ErrorOr<Organization>>;