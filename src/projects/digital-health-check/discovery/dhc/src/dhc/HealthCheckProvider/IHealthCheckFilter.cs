namespace dhc;

public interface IHealthCheckFilter:IHealthCheckProviderFilter
{


    HealthCheckResult Update(HealthCheckResult current, HealthCheckData data);

}


public interface IHandlingInvoker
{
    Task Handle(HealthCheckContext context, HealthCheckResultDelegate next);
}


public interface IHealthCheckProviderFilter : IHandlingInvoker
{
    
}

public abstract class HealthCheckProviderFilter : IHealthCheckProviderFilter
{
    protected readonly ILogger<HealthCheckProviderFilter> _logger;
    public HealthCheckProviderFilter(ILogger<HealthCheckProviderFilter> logger)
    {
        _logger = logger;
    }

    public virtual Task Handle(HealthCheckContext context)
    {
        return Task.CompletedTask;
    }    

    public virtual Task AfterNext(HealthCheckContext context)
    {
        return Task.CompletedTask;
    }

    public async Task Handle(HealthCheckContext context, HealthCheckResultDelegate next)
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