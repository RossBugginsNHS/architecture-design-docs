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

    public override Task Handle(IHealthCheckContext context)
    {
        base._logger.LogInformation("in the health check bmi filter");
        var t = Update(context.HealthCheckResult, context.HealthCheckData);
        context.SetContextObject("HealthCheckFilterBmi_GetBmi_Task", t);
        //Must pair with HealthCheckFilterBmiResult which gets this task and awaits it.
        base._logger.LogInformation("after the health check bmi filter has updated the result");
        return Task.CompletedTask;
    }

    public async Task<HealthCheckResult> Update(HealthCheckResult current, HealthCheckData data)
    {
        var bmi = await _bmiCalculatorProvider.CalculateBmi(data.BasicObs.Height, data.BasicObs.Mass);
        return current with {Bmi = bmi};
    }
}


