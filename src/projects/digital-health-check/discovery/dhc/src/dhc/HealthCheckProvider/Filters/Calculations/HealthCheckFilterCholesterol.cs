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

    public override Task Handle(IHealthCheckContext context)
    {
        context.HealthCheckResult = Update(context.HealthCheckResult, context.HealthCheckData);
        return Task.CompletedTask;
    }

    public HealthCheckResult Update(HealthCheckResult current, HealthCheckData data)
    {
        var cholesterol = _calculator.Calculate(data.CholesterolData);
        return current with {Cholesterol = cholesterol};
    }
}

