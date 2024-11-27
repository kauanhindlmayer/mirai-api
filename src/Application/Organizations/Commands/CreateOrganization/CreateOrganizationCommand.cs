using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.CreateOrganization;

public sealed record CreateOrganizationCommand(
    string Name,
    string Description) : IRequest<ErrorOr<Organization>>;