namespace dhc;

public interface IHealthCheckFilter : IHealthCheckProviderFilter
{
    Task<HealthCheckResult> Update(HealthCheckResult current, HealthCheckData data);
}
