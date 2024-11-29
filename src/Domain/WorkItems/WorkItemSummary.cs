namespace Domain.WorkItems;

public sealed class WorkItemSummary
{
    public Guid Id { get; set; }
    public int Code { get; set; }
    public string Title { get; set; } = string.Empty;
    public double Similarity { get; set; }
}