namespace dhc;

public class HealthCheckGuidanceFilterQrisk: ProviderFilter<IHealthCheckContext>, IHealthCheckGuidanceFilter
{
    private readonly ILogger<HealthCheckGuidanceFilterQrisk> _logger;

    public HealthCheckGuidanceFilterQrisk(
        ILogger<HealthCheckGuidanceFilterQrisk> logger
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
       
            var guidance = current.Guidance;
            var newGuidance = guidance with {QriskGuidance = new HealthCheckResultGuidanceQrisk($"your chance of heart attack in the next 10 years is {Math.Round(current.QriskResult.Result,1)}%")};
            _logger.LogDebug("Setting blood qrisk guidance to {bloodPressureGuidance}", newGuidance.QriskGuidance.Guidance);
            return current with {Guidance = newGuidance};
     
        
    }
}