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
        var t =  _bloodPressureProvider.CalculateBloodPressure(context.HealthCheckData.BloodPressure.Systolic, context.HealthCheckData.BloodPressure.Diastolic, context.CancellationToken);
        context.SetContextObject("HealthCheckFilterBp_GetBp_Task", t);
        return Task.CompletedTask;
    }

    

    public async Task<HealthCheckResult> Update(HealthCheckResult current, HealthCheckData data)
    {
        throw new NotImplementedException();
    }
}