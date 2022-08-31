namespace dhc;

public class HealthCheckContextFactory : IHealthCheckContextFactory
{
    IServiceProvider _sp;
    public HealthCheckContextFactory(IServiceProvider sp)
    {
        _sp = sp;
    }
    public IHealthCheckContext Create()
    {
        var ctx= ActivatorUtilities.GetServiceOrCreateInstance<IHealthCheckContext>(_sp);
        return ctx;
    }
}
