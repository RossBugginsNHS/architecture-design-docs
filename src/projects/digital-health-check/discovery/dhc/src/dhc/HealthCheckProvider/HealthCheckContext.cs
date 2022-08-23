namespace dhc;

public class HealthCheckContext : IHealthCheckContext
{
    public HealthCheckData HealthCheckData { get; set; }
    public HealthCheckResult HealthCheckResult { get; set; }
}
