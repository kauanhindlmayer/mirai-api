using Domain.WorkItems.Enums;

namespace Domain.UnitTests.WorkItems;

public class WorkItemLinkTypeExtensionsTests
{
    [Theory]
    [InlineData(WorkItemLinkType.Related, "Related")]
    [InlineData(WorkItemLinkType.Affects, "Affected By")]
    [InlineData(WorkItemLinkType.Predecessor, "Successor")]
    [InlineData(WorkItemLinkType.Duplicate, "Duplicate Of")]
    public void GetReverseName_ShouldReturnCorrectReverseName(WorkItemLinkType linkType, string expectedReverseName)
    {
        // Act
        var reverseName = linkType.GetReverseName();

        // Assert
        reverseName.Should().Be(expectedReverseName);
    }

    [Theory]
    [InlineData(WorkItemLinkType.Related, "Related")]
    [InlineData(WorkItemLinkType.Affects, "Affects")]
    [InlineData(WorkItemLinkType.Predecessor, "Predecessor")]
    [InlineData(WorkItemLinkType.Duplicate, "Duplicate")]
    public void GetForwardName_ShouldReturnCorrectForwardName(WorkItemLinkType linkType, string expectedForwardName)
    {
        // Act
        var forwardName = linkType.GetForwardName();

        // Assert
        forwardName.Should().Be(expectedForwardName);
    }

    [Theory]
    [InlineData(WorkItemLinkType.Related, true)]
    [InlineData(WorkItemLinkType.Affects, false)]
    [InlineData(WorkItemLinkType.Predecessor, false)]
    [InlineData(WorkItemLinkType.Duplicate, false)]
    public void IsBidirectional_ShouldReturnCorrectValue(WorkItemLinkType linkType, bool expectedIsBidirectional)
    {
        // Act
        var isBidirectional = linkType.IsBidirectional();

        // Assert
        isBidirectional.Should().Be(expectedIsBidirectional);
    }

    [Fact]
    public void GetReverseName_ForRelated_ShouldBeSameAsForwardName()
    {
        // Arrange
        var linkType = WorkItemLinkType.Related;

        // Act
        var forwardName = linkType.GetForwardName();
        var reverseName = linkType.GetReverseName();

        // Assert
        forwardName.Should().Be(reverseName);
        forwardName.Should().Be("Related");
    }
}
