namespace dhc;

public class HealthCheckContextHandler
{
    public Task Handle(HealthCheckContext context)
    {
        return Task.CompletedTask;
    }
}
