using System.Text;

namespace dhc;

public class HealthCheckProvider : IHealthCheckProvider
{
    private readonly PipelineWrapper<IHealthCheckProviderFilter> _pipelineWrapper;
    private readonly IEnumerable<IHealthCheckFilter> _filters;
    private readonly IEnumerable<IHealthCheckGuidanceFilter> _guidanceFilters;

    private readonly ILogger<HealthCheckProvider> _logger;
    public HealthCheckProvider(
        PipelineWrapper<IHealthCheckProviderFilter> pipelineWrapper,
        IEnumerable<IHealthCheckFilter> filters,
        ILogger<HealthCheckProvider> logger,
        IEnumerable<IHealthCheckGuidanceFilter> guidanceFilters)
    {
        _pipelineWrapper = pipelineWrapper;
        _filters = filters;
        _logger = logger;
        _guidanceFilters = guidanceFilters;
    }

    public virtual HealthCheckResult Calculate(HealthCheckData value)
    {
        var current = CalculateResults(value);
        //var result = CalculateGuidance(current, value);
        return current;
    }


    public virtual HealthCheckResult CalculateResults(HealthCheckData value)
    {
         _logger.LogDebug("Starting health check calculation on {healthCheckData}", value);
        var result = _pipelineWrapper.Run(value);
        _logger.LogDebug("Finished health check calculation on {healthCheckData} with result {healthCheckResult}", value, result);
        return result;
    }
}
