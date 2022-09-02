namespace dhc;

public interface IHealthCheckContext
{
    CancellationToken CancellationToken {get;set;}
    HealthCheckData HealthCheckData { get;  }
    HealthCheckResult HealthCheckResult { get;  }

    Guid ContextId {get;}
    
    T GetContextObject<T>(string key);
    void SetContextObject(string key, object value);

    HealthCheckData SetHealthCheckData(HealthCheckData data);

    HealthCheckResult SetHealthCheckResult(HealthCheckResult result);

    IEnumerable<HealthCheckData> GetDataHistory();

    IEnumerable<HealthCheckResult> GetResultsHistory();
}
