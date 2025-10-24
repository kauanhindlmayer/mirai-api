namespace Domain.WorkItems.Enums;

/// <summary>
/// Extension methods for WorkItemLinkType to provide reverse relationship names.
/// </summary>
public static class WorkItemLinkTypeExtensions
{
    /// <summary>
    /// Gets the reverse/inverse name of the link type.
    /// For example, "Predecessor" returns "Successor", "Affects" returns "Affected By".
    /// </summary>
    /// <param name="linkType">The link type to get the reverse name for.</param>
    /// <returns>The reverse relationship name.</returns>
    public static string GetReverseName(this WorkItemLinkType linkType)
    {
        return linkType switch
        {
            WorkItemLinkType.Related => "Related",
            WorkItemLinkType.Affects => "Affected By",
            WorkItemLinkType.Predecessor => "Successor",
            WorkItemLinkType.Duplicate => "Duplicate Of",
            _ => linkType.ToString(),
        };
    }

    /// <summary>
    /// Gets the forward name of the link type.
    /// </summary>
    /// <param name="linkType">The link type to get the forward name for.</param>
    /// <returns>The forward relationship name.</returns>
    public static string GetForwardName(this WorkItemLinkType linkType)
    {
        return linkType switch
        {
            WorkItemLinkType.Related => "Related",
            WorkItemLinkType.Affects => "Affects",
            WorkItemLinkType.Predecessor => "Predecessor",
            WorkItemLinkType.Duplicate => "Duplicate",
            _ => linkType.ToString(),
        };
    }

    /// <summary>
    /// Determines if the link type is bidirectional (symmetric).
    /// </summary>
    /// <param name="linkType">The link type to check.</param>
    /// <returns>True if the link type is bidirectional, false otherwise.</returns>
    public static bool IsBidirectional(this WorkItemLinkType linkType)
    {
        return linkType == WorkItemLinkType.Related;
    }
}
