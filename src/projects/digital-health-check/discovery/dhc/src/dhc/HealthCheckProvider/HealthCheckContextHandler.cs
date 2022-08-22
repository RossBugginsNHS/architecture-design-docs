namespace dhc;

public class HealthCheckContextHandler
{
    public Task Handle(HealthCheckContext context)
    {
        return Task.CompletedTask;
    }
}

public class ContextHandler<T>
{
    public Task Handle(T context)
    {
        return Task.CompletedTask;
    }
}
