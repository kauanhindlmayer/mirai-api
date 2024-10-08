using Application.Common.Behaviors;
using Application.Organizations.Commands.CreateOrganization;
using Domain.Organizations;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using TestCommon.Organizations;

namespace Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<CreateOrganizationCommand, ErrorOr<Organization>> _validationBehavior;
    private readonly IValidator<CreateOrganizationCommand> _mockValidator;
    private readonly RequestHandlerDelegate<ErrorOr<Organization>> _mockNextBehavior;

    public ValidationBehaviorTests()
    {
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<Organization>>>();
        _mockValidator = Substitute.For<IValidator<CreateOrganizationCommand>>();
        _validationBehavior = new(_mockValidator);
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange
        var createOrganizationCommand = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = OrganizationFactory.CreateOrganization();

        _mockValidator
            .ValidateAsync(createOrganizationCommand, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _mockNextBehavior.Invoke().Returns(organization);

        // Act
        var result = await _validationBehavior.Handle(createOrganizationCommand, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(organization);
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenValidatorResultIsNotValid_ShouldReturnListOfErrors()
    {
        // Arrange
        var createOrganizationCommand = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        List<ValidationFailure> validationFailures = [new(propertyName: "foo", errorMessage: "bad foo")];

        _mockValidator
            .ValidateAsync(createOrganizationCommand, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _validationBehavior.Handle(createOrganizationCommand, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("foo");
        result.FirstError.Description.Should().Be("bad foo");
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenNoValidator_ShouldInvokeNextBehavior()
    {
        // Arrange
        var createOrganizationCommand = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var validationBehavior = new ValidationBehavior<CreateOrganizationCommand, ErrorOr<Organization>>();

        var organization = OrganizationFactory.CreateOrganization();
        _mockNextBehavior.Invoke().Returns(organization);

        // Act
        var result = await validationBehavior.Handle(createOrganizationCommand, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(organization);
    }
}