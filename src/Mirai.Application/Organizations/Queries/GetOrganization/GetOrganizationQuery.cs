using ErrorOr;
using MediatR;
using Mirai.Domain.Organizations;

namespace Mirai.Application.Organizations.Queries.GetOrganization;

public record GetOrganizationQuery(Guid OrganizationId)
    : IRequest<ErrorOr<Organization>>;