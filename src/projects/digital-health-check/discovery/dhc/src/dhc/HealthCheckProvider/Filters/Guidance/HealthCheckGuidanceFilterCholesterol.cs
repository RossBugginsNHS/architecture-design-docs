namespace dhc;

public class HealthCheckGuidanceFilterCholesterol: ProviderFilter<IHealthCheckContext>, IHealthCheckGuidanceFilter
{
    private readonly ILogger<HealthCheckGuidanceFilterCholesterol> _logger;

    public HealthCheckGuidanceFilterCholesterol(
        ILogger<HealthCheckGuidanceFilterCholesterol> logger
    ):base(logger)
    {
        _logger = logger;
    }
    public override Task Handle(IHealthCheckContext context)
    {
        context.HealthCheckResult = Update(context.HealthCheckResult, context.HealthCheckData);
        return Task.CompletedTask;
    }

    public HealthCheckResult Update(HealthCheckResult current, HealthCheckData data)
    {
        if(current.Cholesterol.Ratio > 1)
        {
            var guidance = current.Guidance;
            var newGuidance = guidance with {CholesterolGuidance = new CholesterolGuidance("Cholesterol ratio is greater than 1. Is this ok?")};
            _logger.LogDebug("Setting CholesterolGuidance guidance to {CholesterolGuidance}", newGuidance.CholesterolGuidance.Guidance);
            return current with {Guidance = newGuidance};
        }
        else
        {
            return current;
        }
        
    }
}