namespace dhc;

public class HealthCheckGuidanceFilterBp: ProviderFilter<IHealthCheckContext>, IHealthCheckGuidanceFilter
{
    private readonly IBloodPressureProvider _bloodPressureProvider;
    private readonly ILogger<HealthCheckGuidanceFilterBp> _logger;

    public HealthCheckGuidanceFilterBp(
        IBloodPressureProvider bloodPressureProvider,
        ILogger<HealthCheckGuidanceFilterBp> logger
    ):base(logger)
    {
        _bloodPressureProvider = bloodPressureProvider;
        _logger = logger;
    }
    public override Task Handle(IHealthCheckContext context)
    {
        context.SetHealthCheckResult(Update(context.HealthCheckResult, context.HealthCheckData));
        return Task.CompletedTask;
    }

    public HealthCheckResult Update(HealthCheckResult current, HealthCheckData data)
    {
        if(current.BloodPressure.BloodPressureDescription!="Normal")
        {
            var guidance = current.Guidance;
            var newGuidance = guidance with {BloodPressureGuidance = new HealthCheckResultGuidanceBloodPressure("BP is not in the Normal zone, guidance is to think about looking into this.")};
            _logger.LogDebug("Setting blood pressure guidance to {bloodPressureGuidance}", newGuidance.BloodPressureGuidance.Guidance);
            return current with {Guidance = newGuidance};
        }
        else
        {
            return current;
        }
        
    }
}