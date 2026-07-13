using System.Text.RegularExpressions;

namespace Domain.Notifications;

/// <summary>
/// Parses `@mention` user references out of comment HTML - a mention node looks like
/// <c>&lt;span data-type="mention" data-id="{userId}" data-label="..."&gt;@Name&lt;/span&gt;</c>,
/// produced by the frontend's TipTap mention extension.
/// </summary>
public static partial class MentionParser
{
    public static IReadOnlySet<Guid> ParseMentionedUserIds(string html)
    {
        var userIds = new HashSet<Guid>();

        foreach (Match spanMatch in MentionSpanPattern().Matches(html))
        {
            var idMatch = DataIdPattern().Match(spanMatch.Value);
            if (idMatch.Success && Guid.TryParse(idMatch.Groups[1].Value, out var userId))
            {
                userIds.Add(userId);
            }
        }

        return userIds;
    }

    [GeneratedRegex(@"<span[^>]*data-type=""mention""[^>]*>")]
    private static partial Regex MentionSpanPattern();

    [GeneratedRegex(@"data-id=""([^""]+)""")]
    private static partial Regex DataIdPattern();
}
