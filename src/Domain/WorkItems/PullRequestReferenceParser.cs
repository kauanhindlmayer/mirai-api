using System.Text.RegularExpressions;

namespace Domain.WorkItems;

/// <summary>
/// Parses "MB#&lt;code&gt;" work item references out of PR titles, bodies, and
/// branch names.
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

    [GeneratedRegex(@"(?<![\w/])MB#(\d+)\b", RegexOptions.IgnoreCase)]
    private static partial Regex CodeReferencePattern();
}
