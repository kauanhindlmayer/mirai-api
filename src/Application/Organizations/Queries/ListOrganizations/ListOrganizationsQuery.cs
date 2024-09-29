using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Queries.ListOrganizations;

public record ListOrganizationsQuery() : IRequest<ErrorOr<List<Organization>>>;