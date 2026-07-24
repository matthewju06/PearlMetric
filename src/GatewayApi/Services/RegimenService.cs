using PearlMetric.GatewayApi.Contracts.Api.Regimens;
using PearlMetric.GatewayApi.Data;
using PearlMetric.GatewayApi.Models;

namespace PearlMetric.GatewayApi.Services;

public sealed class RegimenService(PearlMetricDb db)
{
    public async Task<RegimenResponse?> CreateAsync(CreateRegimenRequest request, CancellationToken cancellationToken)
    {
        var patientExists = await db.Patients.FindAsync([request.PatientId], cancellationToken) is not null;
        if (!patientExists)
        {
            return null;
        }

        var regimen = new WhiteningRegimen
        {
            Id = Guid.NewGuid(),
            PatientId = request.PatientId,
            ProductName = request.ProductName.Trim(),
            StartedAtUtc = EnsureUtc(request.StartedAtUtc),
            DurationDays = request.DurationDays,
            ScheduledIntervalHours = request.ScheduledIntervalHours
        };

        db.Regimens.Add(regimen);
        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(regimen);
    }

    public async Task<RegimenResponse?> GetByIdAsync(Guid regimenId, CancellationToken cancellationToken)
    {
        var regimen = await db.Regimens.FindAsync([regimenId], cancellationToken);
        return regimen is null ? null : ToResponse(regimen);
    }

    public async Task<RegimenResponse?> UpdateAsync(
        Guid regimenId,
        UpdateRegimenRequest request,
        CancellationToken cancellationToken)
    {
        var regimen = await db.Regimens.FindAsync([regimenId], cancellationToken);
        if (regimen is null)
        {
            return null;
        }

        regimen.ProductName = request.ProductName.Trim();
        regimen.StartedAtUtc = EnsureUtc(request.StartedAtUtc);
        regimen.DurationDays = request.DurationDays;
        regimen.ScheduledIntervalHours = request.ScheduledIntervalHours;

        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(regimen);
    }

    private static DateTime EnsureUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

    private static RegimenResponse ToResponse(WhiteningRegimen regimen) =>
        new(
            regimen.Id,
            regimen.PatientId,
            regimen.ProductName,
            regimen.StartedAtUtc,
            regimen.DurationDays,
            regimen.ScheduledIntervalHours);
}
