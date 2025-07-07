using Domain.Boards.Enums;

namespace Domain.Boards;

public static class BoardColumnTemplates
{
    public static readonly Dictionary<ProcessTemplate, List<ColumnTemplate>> Templates = new()
    {
        {
            ProcessTemplate.Agile, new List<ColumnTemplate>
            {
                new("New"),
                new("Active", 5),
                new("Resolved", 5),
                new("Closed"),
            }
        },
        {
            ProcessTemplate.Scrum, new List<ColumnTemplate>
            {
                new("New"),
                new("Approved", 5),
                new("Committed", 5),
                new("Done"),
            }
        },
        {
            ProcessTemplate.Basic, new List<ColumnTemplate>
            {
                new("To Do"),
                new("Doing", 5),
                new("Done"),
            }
        },
    };
}
