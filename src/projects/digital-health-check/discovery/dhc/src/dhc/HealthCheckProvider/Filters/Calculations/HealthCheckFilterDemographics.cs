namespace dhc;

public class HealthCheckFilterDemographics: ProviderFilter<IHealthCheckContext>,IHealthCheckFilter
{
    private readonly DemographicsProvider _demographic;

    public HealthCheckFilterDemographics(
        DemographicsProvider demographic,
        ILogger<HealthCheckFilterDemographics> logger
    ) : base(logger)
    {
        _demographic = demographic;
    }

    public async override Task Handle(IHealthCheckContext context)
    {
        var newDemo = await _demographic.CalculateAsync(context.HealthCheckData, context.HealthCheckResult.Demographics);
        var result = context.HealthCheckResult with {Demographics = newDemo};
        context.HealthCheckResult = result;
    }

    public HealthCheckResult Update(HealthCheckResult current, HealthCheckData data)
    {
       return current;
    }
}

