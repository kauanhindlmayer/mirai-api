using ErrorOr;
using MediatR;
using Mirai.Domain.Organizations;

namespace Mirai.Application.Organizations.Queries.ListOrganizations;

public record ListOrganizationsQuery() : IRequest<ErrorOr<List<Organization>>>;