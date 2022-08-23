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

    public  override Task Handle(IHealthCheckContext context)
    {
        var result = _qrisk.Calculate(context.HealthCheckData, context.HealthCheckResult);
        var r = context.HealthCheckResult.QriskResult with {Result = result};
        var res = context.HealthCheckResult with {QriskResult = r};
        context.HealthCheckResult = res;

        return Task.CompletedTask;
    }

    public HealthCheckResult Update(HealthCheckResult current, HealthCheckData data)
    {

        return current ;
    }
}

