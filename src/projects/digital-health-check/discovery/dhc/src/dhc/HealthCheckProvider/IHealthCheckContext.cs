namespace dhc;

public interface IHealthCheckContext
{
    HealthCheckData HealthCheckData { get; set; }
    HealthCheckResult HealthCheckResult { get; set; }
}
