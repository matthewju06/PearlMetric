namespace PearlMetric.GatewayApi.Models;

public class ScanFrame
{
    public Guid Id { get; set; }
    public Guid ScanRunId { get; set; }
    public int SequenceIndex { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long ByteSize { get; set; }
    public DateTime CapturedAtUtc { get; set; }
    public string? OriginalFileName { get; set; }

    public ScanRun ScanRun { get; set; } = null!;
}
