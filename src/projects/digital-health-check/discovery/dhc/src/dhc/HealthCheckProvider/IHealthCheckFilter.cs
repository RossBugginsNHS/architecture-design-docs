namespace dhc;

public interface IHealthCheckFilter : IHealthCheckProviderFilter
{
    HealthCheckResult Update(HealthCheckResult current, HealthCheckData data);
}
