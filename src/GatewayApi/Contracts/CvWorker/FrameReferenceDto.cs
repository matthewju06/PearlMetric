namespace PearlMetric.GatewayApi.Contracts.CvWorker;

/// <summary>
/// Ordered frame input for analysis. <see cref="StorageKey"/> is a Gateway-owned key,
/// never an arbitrary external URL.
/// </summary>
public sealed record FrameReferenceDto(
    int SequenceIndex,
    string StorageKey,
    DateTime CapturedAtUtc);
