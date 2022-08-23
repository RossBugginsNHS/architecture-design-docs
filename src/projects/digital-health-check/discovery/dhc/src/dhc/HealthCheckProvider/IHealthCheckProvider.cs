namespace dhc;

public interface IHealthCheckProvider
{
    Task<HealthCheckResult> CalculateAsync(HealthCheckData value);
}
