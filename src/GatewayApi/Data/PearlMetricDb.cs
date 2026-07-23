using Microsoft.EntityFrameworkCore;
using PearlMetric.GatewayApi.Models;

namespace PearlMetric.GatewayApi.Data;

public class PearlMetricDb : DbContext
{
    public PearlMetricDb(DbContextOptions<PearlMetricDb> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<WhiteningRegimen> Regimens => Set<WhiteningRegimen>();
    public DbSet<ScanRun> Runs => Set<ScanRun>();
    public DbSet<ScanFrame> Frames => Set<ScanFrame>();
    public DbSet<ColorMetricSample> ColorMetricSamples => Set<ColorMetricSample>();
    public DbSet<CalibrationProfile> CalibrationProfiles => Set<CalibrationProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Local MVP keeps cascade deletes for convenience. Revisit before production so
        // completed measurement history cannot be wiped by deleting a parent entity.
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(patient => patient.DisplayName)
                .HasMaxLength(Patient.DisplayNameMaxLength)
                .IsRequired();

            entity.Property(patient => patient.CreatedAtUtc)
                .HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<WhiteningRegimen>(entity =>
        {
            entity.Property(regimen => regimen.ProductName)
                .HasMaxLength(WhiteningRegimen.ProductNameMaxLength)
                .IsRequired();

            entity.Property(regimen => regimen.StartedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Regimens_DurationDays",
                    "\"DurationDays\" > 0");
                table.HasCheckConstraint(
                    "CK_Regimens_ScheduledIntervalHours",
                    "\"ScheduledIntervalHours\" > 0");
            });
        });

        modelBuilder.Entity<ScanRun>(entity =>
        {
            entity.Property(run => run.DeviceId)
                .HasMaxLength(ScanRun.DeviceIdMaxLength)
                .IsRequired();

            entity.Property(run => run.CapturedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(run => run.DeltaEFormula)
                .HasMaxLength(ScanRun.DeltaEFormulaMaxLength);

            entity.Property(run => run.AlgorithmVersion)
                .HasMaxLength(ScanRun.AlgorithmVersionMaxLength);

            entity.Property(run => run.FailureReason)
                .HasMaxLength(ScanRun.FailureReasonMaxLength);

            entity.HasOne(run => run.BaselineScanRun)
                .WithMany()
                .HasForeignKey(run => run.BaselineScanRunId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(run => new { run.RegimenId, run.CapturedAtUtc });
        });

        modelBuilder.Entity<ScanFrame>(entity =>
        {
            entity.Property(frame => frame.StorageKey)
                .HasMaxLength(512)
                .IsRequired();

            entity.Property(frame => frame.ContentType)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(frame => frame.OriginalFileName)
                .HasMaxLength(260);

            entity.Property(frame => frame.CapturedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.HasIndex(frame => new { frame.ScanRunId, frame.SequenceIndex })
                .IsUnique();

            entity.HasIndex(frame => frame.StorageKey)
                .IsUnique();

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Frames_SequenceIndex",
                    "\"SequenceIndex\" >= 0");
                table.HasCheckConstraint(
                    "CK_Frames_ByteSize",
                    "\"ByteSize\" > 0");
            });
        });

        modelBuilder.Entity<ColorMetricSample>(entity =>
        {
            entity.Property(sample => sample.CapturedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(sample => sample.ShadeGuideValue)
                .HasMaxLength(ColorMetricSample.ShadeGuideValueMaxLength);

            entity.HasIndex(sample => new { sample.ScanRunId, sample.SequenceIndex })
                .IsUnique();

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_ColorMetricSamples_SequenceIndex",
                    "\"SequenceIndex\" >= 0");
                table.HasCheckConstraint(
                    "CK_ColorMetricSamples_L",
                    "\"L\" >= 0 AND \"L\" <= 100");
                table.HasCheckConstraint(
                    "CK_ColorMetricSamples_A",
                    "\"A\" >= -128 AND \"A\" <= 128");
                table.HasCheckConstraint(
                    "CK_ColorMetricSamples_B",
                    "\"B\" >= -128 AND \"B\" <= 128");
                table.HasCheckConstraint(
                    "CK_ColorMetricSamples_DeltaE",
                    "\"DeltaE\" >= 0");
                table.HasCheckConstraint(
                    "CK_ColorMetricSamples_ConfidenceScore",
                    "\"ConfidenceScore\" >= 0 AND \"ConfidenceScore\" <= 1");
            });
        });

        modelBuilder.Entity<CalibrationProfile>(entity =>
        {
            entity.HasIndex(profile => profile.ScanRunId)
                .IsUnique();

            entity.Property(profile => profile.AlignmentMatrixJson)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(profile => profile.CalibratedAtUtc)
                .HasColumnType("timestamp with time zone");
        });
    }
}
