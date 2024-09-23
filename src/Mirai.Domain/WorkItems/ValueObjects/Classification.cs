using Mirai.Domain.WorkItems.Enums;

namespace Mirai.Domain.WorkItems.ValueObjects;

public class Classification
{
    public ValueAreaType ValueArea { get; set; } = ValueAreaType.Business;
}