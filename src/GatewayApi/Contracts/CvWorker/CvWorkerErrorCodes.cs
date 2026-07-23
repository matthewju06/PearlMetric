namespace PearlMetric.GatewayApi.Contracts.CvWorker;

/// <summary>
/// Stable error codes returned in <see cref="CvWorkerErrorDto.Code"/>.
/// </summary>
public static class CvWorkerErrorCodes
{
    public const string UnsupportedProtocolVersion = "unsupported_protocol_version";
    public const string InvalidRequest = "invalid_request";
    public const string InvalidStorageKey = "invalid_storage_key";
    public const string FrameLimitExceeded = "frame_limit_exceeded";
    public const string ImageNotFound = "image_not_found";
    public const string ImageUnreadable = "image_unreadable";
    public const string CalibrationFailed = "calibration_failed";
    public const string AnalysisFailed = "analysis_failed";
    public const string Cancelled = "cancelled";
}
