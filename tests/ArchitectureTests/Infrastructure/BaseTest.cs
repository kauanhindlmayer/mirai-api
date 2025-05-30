using System.Reflection;
using Application;
using Domain.Common;
using Infrastructure.Persistence;

namespace ArchitectureTests.Infrastructure;

public abstract class BaseTest
{
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IApplicationAssemblyMarker).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(Entity).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
}