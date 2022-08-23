using dhc;

namespace dhcapi;

public class HealthCheckRequestDataConverterProvider : IHealthCheckRequestDataConverterProvider
{
    private readonly IHealthCheckDataBuilderProvider _hcBuilderProvider;

    public HealthCheckRequestDataConverterProvider(IHealthCheckDataBuilderProvider hcBuilderProvider)
    {
        _hcBuilderProvider = hcBuilderProvider;
    }

    public HealthCheckData CovertToDhcHealthCheckData(HealthCheckRequestData data)
    {
        var builder = _hcBuilderProvider.Create();

        builder
            .Age(data.AgeDays)
            .Mass(data.MassKg)
            .Height(data.HeightM)
            .Systolic(data.Systolic)
            .Diastolic(data.Diastolic)
            .Waist(data.WaistM)
            .Postcode(data.Postcode)
            .CigarettesPerDay(data.SmokePerDay)
            .UsedToSmoke(data.UsedToSmoke)
            .CholesterolHDL(data.CholesterolHDL)
            .CholesterolNonHDL(data.CholesterolNonHDL)
            .KeyValue("UtcDateTimeCreated", DateTime.UtcNow);

        var healthCheckData = builder.Build();

        return healthCheckData;
    }


}
