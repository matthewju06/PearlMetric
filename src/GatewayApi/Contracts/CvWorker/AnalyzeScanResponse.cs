namespace PearlMetric.GatewayApi.Contracts.CvWorker;

/// <summary>
/// Successful responses include calibration and ordered samples.
/// Failed responses include <see cref="Error"/> and omit durable calibration/samples.
/// </summary>
public sealed record AnalyzeScanResponse(
    string ProtocolVersion,
    Guid ScanRunId,
    string CorrelationId,
    string IdempotencyKey,
    CvAnalysisStatus Status,
    string AlgorithmVersion,
    DeltaEFormula DeltaEFormula,
    CalibrationResultDto? Calibration,
    IReadOnlyList<ColorMetricResultDto> Samples,
    CvWorkerErrorDto? Error);
