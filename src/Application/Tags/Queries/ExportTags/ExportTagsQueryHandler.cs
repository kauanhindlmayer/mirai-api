using System.Text;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Queries.ExportTags;

internal sealed class ExportTagsQueryHandler
    : IRequestHandler<ExportTagsQuery, ErrorOr<byte[]>>
{
    private readonly ITagRepository _tagRepository;

    public ExportTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<ErrorOr<byte[]>> Handle(
        ExportTagsQuery request,
        CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.ListByProjectAsync(
            request.ProjectId,
            cancellationToken);

        if (tags.Count == 0)
        {
            return TagErrors.NoTagsFound;
        }

        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Id,Name,Description,Color");

        foreach (var tag in tags)
        {
            var line = $"{tag.Id},{Escape(tag.Name)},{Escape(tag.Description)},{tag.Color}";
            csvBuilder.AppendLine(line);
        }

        var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
        return csvBytes;
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}
