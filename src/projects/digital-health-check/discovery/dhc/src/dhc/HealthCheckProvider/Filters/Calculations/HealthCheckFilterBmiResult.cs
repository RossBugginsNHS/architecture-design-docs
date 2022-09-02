namespace dhc;

public class HealthCheckFilterBmiResult: ProviderFilter<IHealthCheckContext>,IHealthCheckFilter
{
    private readonly IBmiCalculatorProvider _bmiCalculatorProvider;

    public HealthCheckFilterBmiResult(
        IBmiCalculatorProvider bmiCalculatorProvider,
        ILogger<HealthCheckFilterBmiResult> logger
    ) : base(logger)
    {
        _bmiCalculatorProvider = bmiCalculatorProvider;
    }

    public async override Task Handle(IHealthCheckContext context)
    {
        base._logger.LogInformation("in the health check bmi result filter");
        var t = context.GetContextObject<Task<Bmi>>("HealthCheckFilterBmi_GetBmi_Task");
        var bmi = await t;
        base._logger.LogInformation("Bmi result into the update filter is {bmi}", bmi);
        base._logger.LogInformation("Before updating context result with Bmi result in the update filter it is {healthCheckResult}", context.HealthCheckResult);
        context.SetHealthCheckResult(context.HealthCheckResult with {Bmi = bmi});
        base._logger.LogInformation("After updating context result with Bmi result in the update filter it is {healthCheckResult}", context.HealthCheckResult);
        base._logger.LogInformation("after the health check bmi filter result has updated the result");

    }

    public  Task<HealthCheckResult> Update(HealthCheckResult current, HealthCheckData data)
    {
        throw new NotImplementedException();
    }
}


