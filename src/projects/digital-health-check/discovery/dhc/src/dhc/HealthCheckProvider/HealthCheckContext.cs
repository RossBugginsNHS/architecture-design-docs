namespace dhc;

public class HealthCheckContext : IHealthCheckContext
{
    public Guid ContextId {get;}
    private readonly ILogger<HealthCheckContext> _logger;
    public HealthCheckData HealthCheckData { get; set; }
    public HealthCheckResult HealthCheckResult { get; set; }

    private readonly  Dictionary<string, object> _context;

    public HealthCheckContext(ILogger<HealthCheckContext> logger)
    {
        ContextId = Guid.NewGuid();
        _logger = logger;
        _context= new Dictionary<string, object>();
    }

    public T GetContextObject<T>(string key)
    {
        _logger.LogDebug("Context {ContextId} getting context key {contextKey}", ContextId, key);
        return (T)_context[key];
    }

    public void SetContextObject(string key, object value)
    {
        _logger.LogDebug("Context {ContextId} setting context key {contextKey} to {contextValue}", ContextId, key, value);
         _context.Add(key, value);
    }    
}
