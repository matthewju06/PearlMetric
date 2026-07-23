namespace PearlMetric.GatewayApi.Contracts.CvWorker;

public sealed record CvWorkerErrorDto(
    string Code,
    string Message,
    string? Detail = null);
