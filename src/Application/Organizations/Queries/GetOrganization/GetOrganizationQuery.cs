using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Queries.GetOrganization;

public record GetOrganizationQuery(Guid OrganizationId)
    : IRequest<ErrorOr<Organization>>;