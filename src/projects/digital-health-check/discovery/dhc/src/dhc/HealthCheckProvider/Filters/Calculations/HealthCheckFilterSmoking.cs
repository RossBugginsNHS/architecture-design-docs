namespace dhc;

public class HealthCheckFilterSmoking: ProviderFilter<IHealthCheckContext>,IHealthCheckFilter
{
    SmokingCalculator _calculator;
    public HealthCheckFilterSmoking(
        ILogger<HealthCheckFilterSmoking> logger,
        SmokingCalculator calculator) : base(logger)
    {
        _calculator = calculator;
    }

    public async override Task Handle(IHealthCheckContext context)
    {
        context.SetHealthCheckResult(await Update(context.HealthCheckResult, context.HealthCheckData));
    }

    public Task<HealthCheckResult> Update(HealthCheckResult current, HealthCheckData data)
    {
        var smoke = _calculator.Calculate(data.SmokingData.CigarettesPerDay, data.SmokingData.UsedToSmoke);
        return Task.FromResult(current with {Smoking = new SmokingResult(data.SmokingData, smoke)});
    }
}

public class SmokingCalculator
{
    public SmokingDescriptionEnum Calculate(int numberPerDayNow, bool usedToSmoke)
    {
        return 
        (numberPerDayNow, usedToSmoke) switch
        {
            (<= 0, true)            => SmokingDescriptionEnum.ExSmoker,
            (0, false)              => SmokingDescriptionEnum.None,
            (> 0 and <= 9, _)   => SmokingDescriptionEnum.Light,
            (> 9 and <= 19, _)  => SmokingDescriptionEnum.Moderate,
            (> 19 and <= 39, _) => SmokingDescriptionEnum.Heavy,
            (> 39, _)           => SmokingDescriptionEnum.VeryHeavy
        };
    }
}
