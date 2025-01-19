using Ardalis.SmartEnum;

namespace Domain.Boards.Enums;

public sealed class ProcessTemplate(string name, int value)
    : SmartEnum<ProcessTemplate>(name, value)
{
    public static readonly ProcessTemplate Basic = new(nameof(Basic), 0);
    public static readonly ProcessTemplate Agile = new(nameof(Agile), 1);
    public static readonly ProcessTemplate Scrum = new(nameof(Scrum), 2);
}