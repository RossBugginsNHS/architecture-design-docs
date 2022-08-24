namespace dhc;

public class HealthCheckFilterBmi: ProviderFilter<IHealthCheckContext>,IHealthCheckFilter
{
    private readonly IBmiCalculatorProvider _bmiCalculatorProvider;

    public HealthCheckFilterBmi(
        IBmiCalculatorProvider bmiCalculatorProvider,
        ILogger<HealthCheckFilterBmi> logger
    ) : base(logger)
    {
        _bmiCalculatorProvider = bmiCalculatorProvider;
    }

    public async override Task Handle(IHealthCheckContext context)
    {
        base._logger.LogInformation("in the health check bmi filter");
        context.HealthCheckResult = await Update(context.HealthCheckResult, context.HealthCheckData);
        base._logger.LogInformation("after the health check bmi filter has updated the result");

    }

    public async Task<HealthCheckResult> Update(HealthCheckResult current, HealthCheckData data)
    {
        var bmi = await _bmiCalculatorProvider.CalculateBmi(data.BasicObs.Height, data.BasicObs.Mass);
        return current with {Bmi = bmi};
    }
}

