namespace dhc;

public class HealthCheckContext : IHealthCheckContext
{
    private Stack<HealthCheckData> _dataHistory = new  Stack<HealthCheckData>();
    private Stack<HealthCheckResult> _resultsHistory = new  Stack<HealthCheckResult>();
    public CancellationToken CancellationToken {get;set;}
    public Guid ContextId {get;}
    private readonly ILogger<HealthCheckContext> _logger;
    public HealthCheckData HealthCheckData { get; private set; }
    public HealthCheckResult HealthCheckResult { get; private set; }

    private readonly  Dictionary<string, object> _context;

    public IEnumerable<HealthCheckData> GetDataHistory()
    {
        return _dataHistory.AsEnumerable();
    }

    public IEnumerable<HealthCheckResult> GetResultsHistory()
    {
        return _resultsHistory.AsEnumerable();
    }

    public HealthCheckData SetHealthCheckData(HealthCheckData data)
    {
        _logger.LogDebug("Setting health check data for context {contextId}", ContextId);
        _dataHistory.Push(data);
        HealthCheckData = data;
        return HealthCheckData;
    }

    public HealthCheckResult SetHealthCheckResult(HealthCheckResult result)
    {
        _logger.LogDebug("Setting health check result for context {contextId}", ContextId);
        _resultsHistory.Push(result);
        HealthCheckResult = result;
        return HealthCheckResult;
    }

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
