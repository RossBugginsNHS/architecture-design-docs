using System.Text;

namespace dhc;

public class HealthCheckProvider : IHealthCheckProvider
{
    private readonly PipelineBuilder<IHealthCheckProviderFilter, IHealthCheckContext> _pipelineWrapper;
    private readonly ILogger<HealthCheckProvider> _logger;
    private readonly IHealthCheckContextBuilder _builder;

    public HealthCheckProvider(
        PipelineBuilder<IHealthCheckProviderFilter, IHealthCheckContext> pipelineWrapper,
        ILogger<HealthCheckProvider> logger,
        IHealthCheckContextBuilder builder)
    {
        _pipelineWrapper = pipelineWrapper;
        _logger = logger;
        _builder = builder;
    }

    public virtual HealthCheckResult Calculate(HealthCheckData value)
    {
        IHealthCheckContext context = _builder.Create();
        context.HealthCheckResult = default(HealthCheckResult);
        context.HealthCheckData = value;
        var current = CalculateResults(context);
        return current;
    }

    public virtual HealthCheckResult CalculateResults(IHealthCheckContext context)
    {
         _logger.LogDebug("Starting health check calculation on {healthCheckData}", context.HealthCheckData);
        var result = _pipelineWrapper.Run(context);
        _logger.LogDebug("Finished health check calculation on {healthCheckData} with result {healthCheckResult}", context.HealthCheckData, context.HealthCheckResult);
        return context.HealthCheckResult;
    }
}
