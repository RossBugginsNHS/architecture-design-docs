namespace dhc;

public interface IHealthCheckGuidanceFilter:IHealthCheckProviderFilter
{
    HealthCheckResult Update(HealthCheckResult current, HealthCheckData data);
}