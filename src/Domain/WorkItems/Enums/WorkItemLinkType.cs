namespace Domain.WorkItems.Enums;

/// <summary>
/// Represents the type of link between work items.
/// </summary>
public enum WorkItemLinkType
{
    /// <summary>
    /// Network topology - bidirectional relationship.
    /// Use to link work items that are at the same level.
    /// </summary>
    Related = 1,

    /// <summary>
    /// Dependency topology - directional relationship.
    /// Forward: "Affects" → Reverse: "Affected By".
    /// Typically used to track change requests made to requirements.
    /// </summary>
    Affects = 2,

    /// <summary>
    /// Dependency topology - directional relationship.
    /// Forward: "Predecessor" → Reverse: "Successor".
    /// Track tasks that must be completed before others can be started.
    /// </summary>
    Predecessor = 3,

    /// <summary>
    /// Tree topology - hierarchical relationship.
    /// Forward: "Duplicate" → Reverse: "Duplicate Of".
    /// Use to track tasks, bugs, or other work items that are duplicates.
    /// </summary>
    Duplicate = 4,
}
