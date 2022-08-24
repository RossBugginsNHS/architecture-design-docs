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

    public async override Task Handle(IHealthCheckContext context)
    {
        context.HealthCheckResult =await Update(context.HealthCheckResult, context.HealthCheckData);
      
    }

    

    public async Task<HealthCheckResult> Update(HealthCheckResult current, HealthCheckData data)
    {
        var bp = await _bloodPressureProvider.CalculateBloodPressure(data.BloodPressure.Systolic, data.BloodPressure.Diastolic);
        return current with {BloodPressure = bp};
    }
}