using PearlMetric.GatewayApi.Contracts.Api.Regimens;
using PearlMetric.GatewayApi.Services;
using PearlMetric.GatewayApi.Validation;

namespace PearlMetric.GatewayApi.Endpoints;

public static class RegimenEndpoints
{
    public static RouteGroupBuilder MapRegimenEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/regimens").WithTags("Regimens");

        group.MapPost("/", async (
                CreateRegimenRequest request,
                RegimenService regimens,
                CancellationToken cancellationToken) =>
            {
                if (RequestValidator.Validate(request) is { } problem)
                {
                    return problem;
                }

                if (request.PatientId == Guid.Empty)
                {
                    return Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["PatientId"] = ["PatientId is required."]
                    });
                }

                var response = await regimens.CreateAsync(request, cancellationToken);
                return response is null
                    ? Results.NotFound(new { title = "Patient not found.", patientId = request.PatientId })
                    : Results.Created($"/api/regimens/{response.Id}", response);
            })
            .WithName("CreateRegimen")
            .Produces<RegimenResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapGet("/{regimenId:guid}", async (
                Guid regimenId,
                RegimenService regimens,
                CancellationToken cancellationToken) =>
            {
                var response = await regimens.GetByIdAsync(regimenId, cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetRegimen")
            .Produces<RegimenResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{regimenId:guid}", async (
                Guid regimenId,
                UpdateRegimenRequest request,
                RegimenService regimens,
                CancellationToken cancellationToken) =>
            {
                if (RequestValidator.Validate(request) is { } problem)
                {
                    return problem;
                }

                var response = await regimens.UpdateAsync(regimenId, request, cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("UpdateRegimen")
            .Produces<RegimenResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        return group;
    }
}
