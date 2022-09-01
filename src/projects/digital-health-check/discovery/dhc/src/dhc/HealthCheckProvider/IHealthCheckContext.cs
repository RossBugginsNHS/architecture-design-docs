namespace dhc;

public interface IHealthCheckContext
{
    CancellationToken CancellationToken {get;set;}
    HealthCheckData HealthCheckData { get; set; }
    HealthCheckResult HealthCheckResult { get; set; }

    Guid ContextId {get;}
    
    T GetContextObject<T>(string key);
    void SetContextObject(string key, object value);
}
