using System.Text.RegularExpressions;

namespace PearlMetric.GatewayApi.Contracts.CvWorker;

/// <summary>
/// Shared CV worker protocol constants. Gateway and Python must honor these limits.
/// </summary>
public static partial class CvWorkerProtocol
{
    public const string Version = "1.0";

    public const int MaxFramesPerRequest = 32;
    public const long MaxFrameBytes = 15 * 1024 * 1024;
    public const int MaxAlignmentMatrixDimension = 4;

    /// <summary>
    /// Storage keys must look like: runs/{scanRunId}/{sequenceIndex}.{ext}
    /// Arbitrary external URLs are rejected.
    /// </summary>
    public const string StorageKeyPattern =
        @"^runs/[0-9a-fA-F-]{36}/[0-9]{1,4}\.(jpg|jpeg|png|webp)$";

    /// <summary>
    /// Gateway HttpClient timeout should match CvWorker:TimeoutSeconds (default 30).
    /// Workers should honor cancellation and stop work promptly when the request is aborted.
    /// </summary>
    public const int DefaultTimeoutSeconds = 30;

    /// <summary>
    /// Safe retries: only for transport failures before a durable result is written.
    /// Clients must reuse the same IdempotencyKey so Gateway does not duplicate samples.
    /// </summary>
    public const int MaxTransientRetries = 2;

    /// <summary>
    /// Partial results are not accepted for MVP. Workers return Success with full samples
    /// or Failed with a structured error and no persisted calibration/samples.
    /// </summary>
    public const bool AllowPartialResults = false;

    public static bool IsSupportedProtocolVersion(string? protocolVersion) =>
        string.Equals(protocolVersion, Version, StringComparison.Ordinal);

    public static bool IsValidStorageKey(string? storageKey) =>
        !string.IsNullOrWhiteSpace(storageKey)
        && !storageKey.Contains("://", StringComparison.Ordinal)
        && StorageKeyRegex().IsMatch(storageKey);

    public static bool IsValidFrameCount(int frameCount) =>
        frameCount is > 0 and <= MaxFramesPerRequest;

    [GeneratedRegex(StorageKeyPattern)]
    private static partial Regex StorageKeyRegex();
}
