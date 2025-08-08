namespace Application.Abstractions.Storage;

public record FileResponse(Stream Stream, string ContentType);