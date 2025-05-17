using CsvHelper.Configuration.Attributes;

namespace Infrastructure.Jobs;

public sealed class CsvTagRecord
{
    [Name("project_id")]
    public Guid ProjectId { get; set; }

    [Name("name")]
    public string Name { get; set; } = string.Empty;

    [Name("description")]
    public string? Description { get; set; }

    [Name("color")]
    public string Color { get; set; } = string.Empty;
}
