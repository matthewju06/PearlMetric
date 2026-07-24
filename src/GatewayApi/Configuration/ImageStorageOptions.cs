namespace PearlMetric.GatewayApi.Configuration;

public sealed class ImageStorageOptions
{
    public const string SectionName = "ImageStorage";

    public string RootPath { get; init; } = string.Empty;
    public long MaxFrameBytes { get; init; } = 15 * 1024 * 1024;
    public int MaxFramesPerRun { get; init; } = 32;
    public long MaxTotalBytesPerRun { get; init; } = 200L * 1024 * 1024;
    public int MinWidth { get; init; } = 64;
    public int MinHeight { get; init; } = 64;
    public int MaxWidth { get; init; } = 8192;
    public int MaxHeight { get; init; } = 8192;
}
