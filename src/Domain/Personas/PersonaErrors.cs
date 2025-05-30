using ErrorOr;

namespace Domain.Personas;

public static class PersonaErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "Persona.NotFound",
        description: "Persona not found.");

    public static readonly Error AlreadyExists = Error.Validation(
        "Persona.AlreadyExists",
        "A persona with the same name already exists in the project.");
}