using System.Text.RegularExpressions;

namespace Domain.WorkItems;

/// <summary>
/// Parses "#&lt;code&gt;" work item references out of PR titles, bodies, and
/// branch names. A connected repository maps 1:1 to a single project, so a
/// bare numeric code is unambiguous once scoped to that project.
/// </summary>
public static partial class PullRequestReferenceParser
{
    public static IReadOnlySet<int> ParseCodes(params string?[] sources)
    {
        var codes = new HashSet<int>();

        foreach (var source in sources)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                continue;
            }

            foreach (Match match in CodeReferencePattern().Matches(source))
            {
                if (int.TryParse(match.Groups[1].Value, out var code))
                {
                    codes.Add(code);
                }
            }
        }

        return codes;
    }

    // Negative lookbehind excludes word characters and "/" immediately before
    // "#" so URL fragments (e.g. "example.com/page#123") and code like
    // "item#123" don't false-match; the trailing \b rejects "#123abc".
    [GeneratedRegex(@"(?<![\w/])#(\d+)\b")]
    private static partial Regex CodeReferencePattern();
}
