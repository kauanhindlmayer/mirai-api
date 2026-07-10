using Application.Abstractions.Authentication;

namespace Application.IntegrationTests.Infrastructure;

/// <summary>
/// Replaces the real <see cref="IUserContext"/> (which reads the current
/// HTTP request's claims and throws outside of one) for tests that call
/// handlers directly through <c>ISender.Send</c> with no HTTP pipeline.
/// </summary>
internal sealed class TestUserContext : IUserContext
{
    public Guid UserId { get; set; } = Guid.Empty;

    public string IdentityId { get; set; } = string.Empty;
}
