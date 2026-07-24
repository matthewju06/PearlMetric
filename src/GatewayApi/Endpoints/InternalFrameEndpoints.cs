using PearlMetric.GatewayApi.Services;

namespace PearlMetric.GatewayApi.Endpoints;

public static class InternalFrameEndpoints
{
    public static IEndpointRouteBuilder MapInternalFrameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/internal/frames")
            .WithTags("Internal")
            .ExcludeFromDescription();

        group.MapGet("/{runId:guid}/{sequenceIndex:int}", async (
                Guid runId,
                int sequenceIndex,
                FrameUploadService frames,
                CancellationToken cancellationToken) =>
            {
                var opened = await frames.OpenFrameAsync(runId, sequenceIndex, cancellationToken);
                if (opened is null)
                {
                    return Results.NotFound();
                }

                var (frame, stream) = opened.Value;
                return Results.File(stream, frame.ContentType, enableRangeProcessing: false);
            })
            .WithName("GetInternalFrame");

        return app;
    }
}
