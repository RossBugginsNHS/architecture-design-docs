namespace dhc;

public class HealthCheckFilterBmi: HealthCheckProviderFilter,IHealthCheckFilter
{
    private readonly IBmiCalculatorProvider _bmiCalculatorProvider;

    public HealthCheckFilterBmi(
        IBmiCalculatorProvider bmiCalculatorProvider,
        ILogger<HealthCheckFilterBmi> logger
    ) : base(logger)
    {
        _bmiCalculatorProvider = bmiCalculatorProvider;
    }

    public override Task Handle(HealthCheckContext context)
    {
        context.HealthCheckResult = Update(context.HealthCheckResult, context.HealthCheckData);
        return Task.CompletedTask;
    }

    public HealthCheckResult Update(HealthCheckResult current, HealthCheckData data)
    {
        var bmi = _bmiCalculatorProvider.CalculateBmi(data.BasicObs.Height, data.BasicObs.Mass);
        return current with {Bmi = bmi};
    }
}

