namespace Application.Abstractions;

public sealed class LinkResponse
{
    public required string Href { get; init; }
    public required string Rel { get; init; }
    public required string Method { get; init; }
}
