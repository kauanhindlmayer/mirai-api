namespace Contracts.Tags;

public sealed record UpdateTagRequest(
    string Name,
    string Description,
    string Color);