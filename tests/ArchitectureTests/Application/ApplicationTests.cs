using ArchitectureTests.Infrastructure;
using FluentAssertions;
using FluentValidation;
using MediatR;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace ArchitectureTests.Application;

public class ApplicationTests : BaseTest
{
    [Fact]
    public void RequestHandler_Should_NotBePublic()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IRequestHandler<>))
            .Or()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Validator_ShouldHave_NameEndingWith_Validator()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .HaveNameEndingWith("Validator")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Validator_Should_NotBePublic()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .NotBePublic()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
