using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PearlMetric.GatewayApi.Configuration;
using PearlMetric.GatewayApi.Contracts.Api.Frames;
using PearlMetric.GatewayApi.Data;
using PearlMetric.GatewayApi.Models;
using PearlMetric.GatewayApi.Storage;

namespace PearlMetric.GatewayApi.Services;

public sealed class FrameUploadService(
    PearlMetricDb db,
    IImageStore imageStore,
    IOptions<ImageStorageOptions> options)
{
    private readonly ImageStorageOptions _options = options.Value;

    public async Task<RegisterFramesResponse?> UploadAsync(
        Guid runId,
        IReadOnlyList<IFormFile> files,
        DateTime? capturedAtUtc,
        CancellationToken cancellationToken)
    {
        var run = await db.Runs
            .Include(item => item.Frames)
            .FirstOrDefaultAsync(item => item.Id == runId, cancellationToken);

        if (run is null)
        {
            return null;
        }

        if (files.Count == 0)
        {
            throw new ImageValidationException("At least one image file is required.");
        }

        if (files.Count > _options.MaxFramesPerRun)
        {
            throw new ImageValidationException(
                $"A run accepts at most {_options.MaxFramesPerRun} frames.");
        }

        if (run.Frames.Count + files.Count > _options.MaxFramesPerRun)
        {
            throw new ImageValidationException(
                $"Uploading these files would exceed the {_options.MaxFramesPerRun} frame limit for the run.");
        }

        var existingBytes = run.Frames.Sum(frame => frame.ByteSize);
        var nextSequence = run.Frames.Count == 0
            ? 0
            : run.Frames.Max(frame => frame.SequenceIndex) + 1;

        var captured = EnsureUtc(capturedAtUtc ?? DateTime.UtcNow);
        var created = new List<ScanFrame>();

        for (var index = 0; index < files.Count; index++)
        {
            var file = files[index];
            var sequenceIndex = nextSequence + index;

            await using var stream = file.OpenReadStream();
            var stored = await imageStore.SaveAsync(
                runId,
                sequenceIndex,
                stream,
                file.FileName,
                cancellationToken);

            if (existingBytes + stored.ByteSize > _options.MaxTotalBytesPerRun)
            {
                throw new ImageValidationException(
                    $"Upload would exceed the per-run storage quota of {_options.MaxTotalBytesPerRun} bytes.");
            }

            existingBytes += stored.ByteSize;

            var frame = new ScanFrame
            {
                Id = Guid.NewGuid(),
                ScanRunId = runId,
                SequenceIndex = sequenceIndex,
                StorageKey = stored.StorageKey,
                ContentType = stored.ContentType,
                ByteSize = stored.ByteSize,
                CapturedAtUtc = captured,
                OriginalFileName = SanitizeOriginalFileName(file.FileName)
            };

            db.Frames.Add(frame);
            created.Add(frame);
        }

        await db.SaveChangesAsync(cancellationToken);

        return new RegisterFramesResponse(
            runId,
            created.Select(ToResponse).ToArray());
    }

    public async Task<(ScanFrame Frame, Stream Content)?> OpenFrameAsync(
        Guid runId,
        int sequenceIndex,
        CancellationToken cancellationToken)
    {
        var frame = await db.Frames
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.ScanRunId == runId && item.SequenceIndex == sequenceIndex,
                cancellationToken);

        if (frame is null)
        {
            return null;
        }

        var stream = await imageStore.OpenReadAsync(frame.StorageKey, cancellationToken);
        if (stream is null)
        {
            return null;
        }

        return (frame, stream);
    }

    private static string? SanitizeOriginalFileName(string? originalFileName)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            return null;
        }

        var name = Path.GetFileName(originalFileName.Trim());
        return name.Length <= 260 ? name : name[..260];
    }

    private static DateTime EnsureUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

    private static FrameResponse ToResponse(ScanFrame frame) =>
        new(
            frame.Id,
            frame.ScanRunId,
            frame.SequenceIndex,
            frame.StorageKey,
            frame.ContentType,
            frame.ByteSize,
            frame.CapturedAtUtc,
            frame.OriginalFileName);
}
