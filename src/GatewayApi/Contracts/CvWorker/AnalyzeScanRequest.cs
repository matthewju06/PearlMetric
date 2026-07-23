namespace PearlMetric.GatewayApi.Contracts.CvWorker;

/// <summary>
/// Request body for synchronous scan analysis.
/// When <see cref="Baseline"/> is null, this scan is treated as the regimen baseline and
/// every returned sample DeltaE must be 0.
/// </summary>
public sealed record AnalyzeScanRequest(
    string ProtocolVersion,
    Guid ScanRunId,
    string CorrelationId,
    string IdempotencyKey,
    IReadOnlyList<FrameReferenceDto> Frames,
    BaselineColorDto? Baseline,
    DeltaEFormula DeltaEFormula = DeltaEFormula.CIEDE2000);
