namespace Contracts.Dashboards;

public sealed record GetDashboardRequest
{
    /// <summary>
    /// The start date for filtering. If not provided, the default is the
    /// current date.
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// The end date for filtering. If not provided, the default is the current
    /// date plus 14 days.
    /// </summary>
    public DateTime EndDate { get; init; }
}