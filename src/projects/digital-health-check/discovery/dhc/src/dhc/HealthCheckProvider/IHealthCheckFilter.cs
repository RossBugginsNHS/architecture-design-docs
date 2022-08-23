namespace dhc;

public interface IHealthCheckFilter : IHealthCheckProviderFilter
{
    HealthCheckResult Update(HealthCheckResult current, HealthCheckData data);
}


public interface IHealthCheckProviderFilter : IHandlingInvoker<IHealthCheckContext>
{

}
