using Domain.WorkItems.Enums;

namespace Domain.WorkItems.ValueObjects;

public class Classification
{
    public ValueAreaType ValueArea { get; set; } = ValueAreaType.Business;
}