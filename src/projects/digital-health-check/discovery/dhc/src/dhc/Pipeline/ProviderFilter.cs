namespace dhc;

public abstract class ProviderFilter<T> : IHandlingInvoker<T>
{
    protected readonly ILogger<ProviderFilter<T>> _logger;
    public ProviderFilter(ILogger<ProviderFilter<T>> logger)
    {
        _logger = logger;

    }

    public virtual Task Handle(T context)
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterNext(T context)
    {
        return Task.CompletedTask;
    }

    public async Task Handle(T context, ContextDelegate<T> next)
    {
        _logger.LogDebug("Before calling Concrete Handle in {className}", this.GetType().Name);

        await Handle(context);

        _logger.LogDebug("After calling Concrete Handle in {className}", this.GetType().Name);

        _logger.LogDebug("Before calling Next in {className}", this.GetType().Name);
        await next(context);
        await AfterNext(context);
        _logger.LogDebug("Afer calling Next in {className}", this.GetType().Name);
    }


}