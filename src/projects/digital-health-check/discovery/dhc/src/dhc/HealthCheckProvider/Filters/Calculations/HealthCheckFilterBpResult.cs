namespace dhc;

public class HealthCheckFilterBpResult: ProviderFilter<IHealthCheckContext>,IHealthCheckFilter
{
    private readonly IBloodPressureProvider _bloodPressureProvider;

    public HealthCheckFilterBpResult(
        IBloodPressureProvider bloodPressureProvider,
        ILogger<HealthCheckFilterBpResult> logger
    ): base (logger)
    {
        _bloodPressureProvider = bloodPressureProvider;
    }

    public async override Task Handle(IHealthCheckContext context)
    {
        var t = context.GetContextObject<Task<BloodPressure>>("HealthCheckFilterBp_GetBp_Task");
        var bp = await t;
        context.HealthCheckResult = context.HealthCheckResult  with {BloodPressure = bp};      
    }

    

    public  Task<HealthCheckResult> Update(HealthCheckResult current, HealthCheckData data)
    {
   throw new NotImplementedException();
    }
}