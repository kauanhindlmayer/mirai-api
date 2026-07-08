namespace Domain.WorkItems.Enums;

/// <summary>
/// Represents the lifecycle state of a linked GitHub pull request.
/// </summary>
public enum PullRequestLinkState
{
    Open = 1,
    Merged = 2,
    Closed = 3,
}
