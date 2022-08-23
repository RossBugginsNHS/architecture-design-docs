namespace dhc;

public class HealthCheckContext : IHealthCheckContext
{
    public HealthCheckData HealthCheckData { get; set; }
    public HealthCheckResult HealthCheckResult { get; set; }
}

public class HealthCheckContextBuilder : IHealthCheckContextBuilder
{
    IServiceProvider _sp;
    public HealthCheckContextBuilder(IServiceProvider sp)
    {
        _sp = sp;
    }
    public IHealthCheckContext Create()
    {
        return ActivatorUtilities.GetServiceOrCreateInstance<IHealthCheckContext>(_sp);

    }
}

public interface IHealthCheckContextBuilder
{
     IHealthCheckContext Create();
}