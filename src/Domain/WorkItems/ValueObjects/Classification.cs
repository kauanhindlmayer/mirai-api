using Domain.WorkItems.Enums;

namespace Domain.WorkItems.ValueObjects;

public sealed class Classification
{
    public ValueAreaType ValueArea { get; set; } = ValueAreaType.Business;
}