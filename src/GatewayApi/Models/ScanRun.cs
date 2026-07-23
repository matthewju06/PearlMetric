namespace PearlMetric.GatewayApi.Models;

public class ScanRun
{
    public const int DeviceIdMaxLength = 100;
    public const int AlgorithmVersionMaxLength = 64;
    public const int DeltaEFormulaMaxLength = 32;
    public const int FailureReasonMaxLength = 1000;

    public Guid Id { get; set; }
    public Guid RegimenId { get; set; }
    public DateTime CapturedAtUtc { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public ScanRunStatus Status { get; set; }

    /// <summary>
    /// First completed scan in the regimen used as the DeltaE baseline, when available.
    /// Null for the regimen's baseline scan itself.
    /// </summary>
    public Guid? BaselineScanRunId { get; set; }

    public string? DeltaEFormula { get; set; }
    public string? AlgorithmVersion { get; set; }
    public string? FailureReason { get; set; }

    public WhiteningRegimen Regimen { get; set; } = null!;
    public ScanRun? BaselineScanRun { get; set; }
    public ICollection<ScanFrame> Frames { get; set; } = new List<ScanFrame>();
    public ICollection<ColorMetricSample> Samples { get; set; } = new List<ColorMetricSample>();
    public CalibrationProfile? Calibration { get; set; }
}
