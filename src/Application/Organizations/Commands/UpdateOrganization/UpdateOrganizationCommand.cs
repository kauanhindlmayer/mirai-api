using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.UpdateOrganization;

public sealed record UpdateOrganizationCommand(
    Guid Id,
    string Name,
    string Description) : IRequest<ErrorOr<Guid>>;