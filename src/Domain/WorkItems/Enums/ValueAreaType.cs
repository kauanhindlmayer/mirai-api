using Ardalis.SmartEnum;

namespace Domain.WorkItems.Enums;

public sealed class ValueAreaType(string name, int value)
    : SmartEnum<ValueAreaType>(name, value)
{
    public static readonly ValueAreaType Architectural = new(nameof(Architectural), 0);
    public static readonly ValueAreaType Business = new(nameof(Business), 1);
}