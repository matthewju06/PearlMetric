using System.Text.Json.Serialization;

namespace PearlMetric.GatewayApi.Contracts.CvWorker;

[JsonConverter(typeof(JsonStringEnumConverter<CvAnalysisStatus>))]
public enum CvAnalysisStatus
{
    Success,
    Failed
}
