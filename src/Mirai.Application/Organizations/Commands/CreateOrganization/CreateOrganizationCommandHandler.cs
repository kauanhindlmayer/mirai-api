using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Organizations;

namespace Mirai.Application.Organizations.Commands.CreateOrganization;

public class CreateOrganizationCommandHandler(IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<CreateOrganizationCommand, ErrorOr<Organization>>
{
    public async Task<ErrorOr<Organization>> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var organization = new Organization(request.Name, request.Description);

        await _organizationsRepository.AddAsync(organization, cancellationToken);

        return organization;
    }
}