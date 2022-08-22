using System.Text;

namespace dhc;

public class HealthCheckProvider : IHealthCheckProvider
{
    private readonly PipelineWrapper<IHealthCheckProviderFilter, HealthCheckContext> _pipelineWrapper;
    private readonly IEnumerable<IHealthCheckFilter> _filters;
    private readonly IEnumerable<IHealthCheckGuidanceFilter> _guidanceFilters;

    private readonly ILogger<HealthCheckProvider> _logger;
    public HealthCheckProvider(
        PipelineWrapper<IHealthCheckProviderFilter,HealthCheckContext> pipelineWrapper,
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
        HealthCheckContext context = new HealthCheckContext();
        context.HealthCheckResult = default(HealthCheckResult);
        context.HealthCheckData = value;

        var current = CalculateResults(context);
        //var result = CalculateGuidance(current, value);
        return current;
    }


    public virtual HealthCheckResult CalculateResults(HealthCheckContext context)
    {
         _logger.LogDebug("Starting health check calculation on {healthCheckData}", context.HealthCheckData);
        var result = _pipelineWrapper.Run(context);
        _logger.LogDebug("Finished health check calculation on {healthCheckData} with result {healthCheckResult}", context.HealthCheckData, context.HealthCheckResult);
        return context.HealthCheckResult;
    }
}
