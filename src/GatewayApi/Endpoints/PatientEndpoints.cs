using PearlMetric.GatewayApi.Contracts.Api.Patients;
using PearlMetric.GatewayApi.Services;
using PearlMetric.GatewayApi.Validation;

namespace PearlMetric.GatewayApi.Endpoints;

public static class PatientEndpoints
{
    public static RouteGroupBuilder MapPatientEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/patients").WithTags("Patients");

        group.MapPost("/", async (
                CreatePatientRequest request,
                PatientService patients,
                CancellationToken cancellationToken) =>
            {
                if (RequestValidator.Validate(request) is { } problem)
                {
                    return problem;
                }

                var response = await patients.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/patients/{response.Id}", response);
            })
            .WithName("CreatePatient")
            .Produces<PatientResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/{patientId:guid}", async (
                Guid patientId,
                PatientService patients,
                CancellationToken cancellationToken) =>
            {
                var response = await patients.GetByIdAsync(patientId, cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetPatient")
            .Produces<PatientResponse>()
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
