namespace dhc;

public class HealthCheckFilterBp: ProviderFilter<IHealthCheckContext>,IHealthCheckFilter
{
    private readonly IBloodPressureProvider _bloodPressureProvider;

    public HealthCheckFilterBp(
        IBloodPressureProvider bloodPressureProvider,
        ILogger<HealthCheckFilterBp> logger
    ): base (logger)
    {
        _bloodPressureProvider = bloodPressureProvider;
    }

    public override Task Handle(IHealthCheckContext context)
    {
        context.HealthCheckResult = Update(context.HealthCheckResult, context.HealthCheckData);
        return Task.CompletedTask;
    }

    

    public HealthCheckResult Update(HealthCheckResult current, HealthCheckData data)
    {
        var bp = _bloodPressureProvider.CalculateBloodPressure(data.BloodPressure.Systolic, data.BloodPressure.Diastolic);
        return current with {BloodPressure = bp};
    }
}