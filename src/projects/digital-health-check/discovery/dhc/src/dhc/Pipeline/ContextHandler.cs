namespace dhc;

public class ContextHandler<T>
{
    public Task Handle(T context)
    {
        return Task.CompletedTask;
    }
}
