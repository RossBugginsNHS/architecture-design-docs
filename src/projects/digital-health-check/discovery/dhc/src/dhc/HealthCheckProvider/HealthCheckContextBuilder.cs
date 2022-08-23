namespace dhc;

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
