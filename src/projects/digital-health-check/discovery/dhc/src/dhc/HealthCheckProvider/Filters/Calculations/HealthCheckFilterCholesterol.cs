namespace dhc;

public class HealthCheckFilterCholesterol: ProviderFilter<IHealthCheckContext>,IHealthCheckFilter
{
    private readonly ICholesterolCalculatorProvider _calculator;
    public HealthCheckFilterCholesterol(
         ICholesterolCalculatorProvider calculator,
        ILogger<HealthCheckFilterCholesterol> logger
    ) : base(logger)
    {
      _calculator = calculator;
    }

    public async override Task Handle(IHealthCheckContext context)
    {
        context.SetHealthCheckResult(await Update(context.HealthCheckResult, context.HealthCheckData));
    }

    public async Task<HealthCheckResult> Update(HealthCheckResult current, HealthCheckData data)
    {
        var cholesterol = await _calculator.Calculate(data.CholesterolData);
        return current with {Cholesterol = cholesterol};
    }
}

