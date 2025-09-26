using Domain.Retrospectives;

namespace Domain.UnitTests.Retrospectives;

internal static class RetrospectiveColumnFactory
{
    public static readonly string Title = "Column Title";
    public static readonly Guid RetrospectiveId = Guid.NewGuid();
    public static readonly Guid AuthorId = Guid.NewGuid();

    public static RetrospectiveColumn Create(
        string? title = null,
        Guid? retrospectiveId = null)
    {
        return new RetrospectiveColumn(
            title ?? Title,
            retrospectiveId ?? RetrospectiveId);
    }

    public static RetrospectiveItem CreateItem(
        string? content = null,
        Guid? columnId = null,
        Guid? authorId = null,
        int? position = null)
    {
        var item = new RetrospectiveItem(
            content ?? $"Item {Guid.NewGuid()}",
            columnId ?? Guid.NewGuid(),
            authorId ?? AuthorId);

        if (position.HasValue)
        {
            item.UpdatePosition(position.Value);
        }

        return item;
    }
}
