using Domain.Personas;

namespace Domain.UnitTests.Personas;

internal static class PersonaFactory
{
    public const string Name = "Test Persona";
    public const string Category = "Test Category";
    public const string Description = "Test Description";
    public static readonly Guid ProjectId = Guid.NewGuid();

    public static Persona Create(
        Guid? projectId = null,
        string? name = null,
        string? category = null,
        string? description = null)
    {
        return new(
            projectId ?? ProjectId,
            name ?? Name,
            category ?? Category,
            description ?? Description);
    }
}