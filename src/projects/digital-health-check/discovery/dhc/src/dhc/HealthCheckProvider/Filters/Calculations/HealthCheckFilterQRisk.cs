namespace dhc;

public class HealthCheckFilterQRisk: ProviderFilter<IHealthCheckContext>,IHealthCheckFilter
{
    private readonly QRiskProvider _qrisk;

    public HealthCheckFilterQRisk(
        QRiskProvider qrisk,
        ILogger<HealthCheckFilterQRisk> logger
    ) : base(logger)
    {
        _qrisk = qrisk;
    }

    public async override Task Handle(IHealthCheckContext context)
    {
        await _qrisk.CalculateAsync(context.HealthCheckData, context.HealthCheckResult);
        context.HealthCheckResult = Update(context.HealthCheckResult, context.HealthCheckData);
    }

    public HealthCheckResult Update(HealthCheckResult current, HealthCheckData data)
    {

        return current ;
    }
}

