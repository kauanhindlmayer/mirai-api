using Domain.Common;
using Domain.WorkItems.Enums;

namespace Domain.WorkItems.ValueObjects;

public sealed class Classification : ValueObject
{
    public ValueAreaType ValueArea { get; set; } = ValueAreaType.Business;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueArea;
    }
}