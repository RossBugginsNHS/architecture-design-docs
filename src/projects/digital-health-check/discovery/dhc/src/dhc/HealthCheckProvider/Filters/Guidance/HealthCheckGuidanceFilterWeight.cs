namespace dhc;

public class HealthCheckGuidanceFilterWeight: ProviderFilter<HealthCheckContext>,IHealthCheckGuidanceFilter
{

    private readonly ILogger<HealthCheckGuidanceFilterWeight> _logger;
    public HealthCheckGuidanceFilterWeight(
        ILogger<HealthCheckGuidanceFilterWeight> logger
    ):base(logger)
    {
        _logger = logger;
    }

    public override Task Handle(HealthCheckContext context)
    {
        context.HealthCheckResult = Update(context.HealthCheckResult, context.HealthCheckData);
        return Task.CompletedTask;
    }

    public HealthCheckResult Update(HealthCheckResult current, HealthCheckData data)
    {
        if(current.Bmi.BmiDescription!=BmiEnum.Healthy)
        {
            var guidance = current.Guidance;
            var newGuidance = guidance with {WeightGuidance = new HealthCheckResultGuidanceWeight("Weight is not in the Normal zone, guidance is to think about looking into this.")};
            _logger.LogDebug("Setting weight guidance to {weightGuidance}", newGuidance.WeightGuidance.Guidance);
            return current with {Guidance = newGuidance};
        }
        else
        {
            return current;
        }
        
    }
}