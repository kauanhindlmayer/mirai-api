namespace Application.Personas.Queries.ListPersonas;

public sealed class PersonaBriefResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? AvatarUrl { get; init; }
}