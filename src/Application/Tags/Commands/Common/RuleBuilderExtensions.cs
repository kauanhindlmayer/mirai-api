using System.Text.RegularExpressions;
using FluentValidation;

namespace Application.Tags.Validation;

public static partial class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeAValidColor<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(color => HexColorRegex().IsMatch(color))
            .WithMessage("Invalid color format. Must be a valid hex color (e.g., #FFFFFF or #FFF).");
    }

    [GeneratedRegex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")]
    private static partial Regex HexColorRegex();
}
