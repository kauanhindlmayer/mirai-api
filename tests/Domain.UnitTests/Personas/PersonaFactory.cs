using Domain.Personas;

namespace Domain.UnitTests.Personas;

public static class PersonaFactory
{
    public const string Name = "Test Persona";
    public const string Category = "Test Category";
    public const string Description = "Test Description";
    public static readonly Guid ProjectId = Guid.NewGuid();

    public static Persona CreatePersona(
        Guid? projectId = null,
        string name = Name,
        string category = Category,
        string description = Description)
    {
        return new(
            projectId ?? ProjectId,
            name,
            category,
            description);
    }
}