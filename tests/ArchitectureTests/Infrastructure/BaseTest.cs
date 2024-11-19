using System.Reflection;
using Application;
using Domain.Common;
using Infrastructure.Persistence;
using WebApi;

namespace ArchitectureTests.Infrastructure;

public abstract class BaseTest
{
    protected static readonly Assembly PresentationAssembly = typeof(IWebApiAssemblyMarker).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IApplicationAssemblyMarker).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(Entity).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
}