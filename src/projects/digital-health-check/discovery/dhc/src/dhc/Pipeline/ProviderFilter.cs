namespace dhc;

public abstract class ProviderFilter<T> : IHandlingInvoker<T>
{
    private static readonly Counter _c_provider_filter_run_count =
    Metrics.CreateCounter("provider_filter_run_count", "how many times pipeline provider filter has run.",
     new CounterConfiguration
     {
         // Here you specify only the names of the labels.
         LabelNames = new[] { "filter_name" }
     });

     private readonly Type _type;
    protected readonly ILogger<ProviderFilter<T>> _logger;
    public ProviderFilter(ILogger<ProviderFilter<T>> logger)
    {
        _type = this.GetType();
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
        _c_provider_filter_run_count.WithLabels(_type.FullName).Inc();

        _logger.LogDebug("After calling Concrete Handle in {className}", this.GetType().Name);

        _logger.LogDebug("Before calling Next in {className}", this.GetType().Name);
        await next(context);
        await AfterNext(context);
        _logger.LogDebug("Afer calling Next in {className}", this.GetType().Name);
    }


}