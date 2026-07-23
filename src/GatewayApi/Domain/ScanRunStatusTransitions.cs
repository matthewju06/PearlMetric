using PearlMetric.GatewayApi.Models;

namespace PearlMetric.GatewayApi.Domain;

public static class ScanRunStatusTransitions
{
    private static readonly IReadOnlyDictionary<ScanRunStatus, HashSet<ScanRunStatus>> Allowed =
        new Dictionary<ScanRunStatus, HashSet<ScanRunStatus>>
        {
            [ScanRunStatus.Pending] = [ScanRunStatus.Processing, ScanRunStatus.Failed],
            [ScanRunStatus.Processing] = [ScanRunStatus.Completed, ScanRunStatus.Failed],
            [ScanRunStatus.Completed] = [],
            [ScanRunStatus.Failed] = [ScanRunStatus.Pending]
        };

    public static bool CanTransition(ScanRunStatus from, ScanRunStatus to) =>
        Allowed.TryGetValue(from, out var next) && next.Contains(to);

    public static void EnsureCanTransition(ScanRunStatus from, ScanRunStatus to)
    {
        if (!CanTransition(from, to))
        {
            throw new InvalidOperationException(
                $"Invalid scan-run status transition from '{from}' to '{to}'.");
        }
    }
}
