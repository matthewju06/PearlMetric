using PearlMetric.GatewayApi.Contracts.Api.Patients;
using PearlMetric.GatewayApi.Data;
using PearlMetric.GatewayApi.Models;

namespace PearlMetric.GatewayApi.Services;

public sealed class PatientService(PearlMetricDb db)
{
    public async Task<PatientResponse> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Patients.Add(patient);
        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(patient);
    }

    public async Task<PatientResponse?> GetByIdAsync(Guid patientId, CancellationToken cancellationToken)
    {
        var patient = await db.Patients.FindAsync([patientId], cancellationToken);
        return patient is null ? null : ToResponse(patient);
    }

    private static PatientResponse ToResponse(Patient patient) =>
        new(patient.Id, patient.DisplayName, patient.CreatedAtUtc);
}
