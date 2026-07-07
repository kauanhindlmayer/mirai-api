using Domain.WorkItems;

namespace Domain.UnitTests.WorkItems;

public class PullRequestReferenceParserTests
{
    [Fact]
    public void ParseCodes_WithSingleReference_ShouldReturnCode()
    {
        // Act
        var codes = PullRequestReferenceParser.ParseCodes("Fixes #42");

        // Assert
        codes.Should().BeEquivalentTo([42]);
    }

    [Fact]
    public void ParseCodes_WithMultipleReferencesAcrossSources_ShouldReturnAllDistinctCodes()
    {
        // Act
        var codes = PullRequestReferenceParser.ParseCodes(
            "Fixes #42 and #43",
            "Also relates to #44, see #42 again",
            "feature/44-fix-branch");

        // Assert
        codes.Should().BeEquivalentTo([42, 43, 44]);
    }

    [Fact]
    public void ParseCodes_WithNearMissTrailingLetters_ShouldNotMatch()
    {
        // Act
        var codes = PullRequestReferenceParser.ParseCodes("See #123abc for context");

        // Assert
        codes.Should().BeEmpty();
    }

    [Fact]
    public void ParseCodes_WithUrlFragment_ShouldNotMatch()
    {
        // Act
        var codes = PullRequestReferenceParser.ParseCodes(
            "See https://example.com/page#123 for details");

        // Assert
        codes.Should().BeEmpty();
    }

    [Fact]
    public void ParseCodes_WithReferenceInParentheses_ShouldMatch()
    {
        // Act
        var codes = PullRequestReferenceParser.ParseCodes("Closes issue (#7)");

        // Assert
        codes.Should().BeEquivalentTo([7]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("No references here")]
    public void ParseCodes_WithNoReferences_ShouldReturnEmptySet(string? source)
    {
        // Act
        var codes = PullRequestReferenceParser.ParseCodes(source);

        // Assert
        codes.Should().BeEmpty();
    }

    [Fact]
    public void ParseCodes_WithNoSources_ShouldReturnEmptySet()
    {
        // Act
        var codes = PullRequestReferenceParser.ParseCodes();

        // Assert
        codes.Should().BeEmpty();
    }
}
