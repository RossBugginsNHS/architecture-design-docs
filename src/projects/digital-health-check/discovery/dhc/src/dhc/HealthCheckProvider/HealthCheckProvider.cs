using System.Text;

namespace dhc;

public class HealthCheckProvider : IHealthCheckProvider
{
    private readonly IPipelineBuilder<IHealthCheckProviderFilter, IHealthCheckContext> _pipelineWrapper;
    private readonly ILogger<HealthCheckProvider> _logger;
    private readonly IHealthCheckContextBuilder _builder;

    public HealthCheckProvider(
        IPipelineBuilder<IHealthCheckProviderFilter, IHealthCheckContext> pipelineWrapper,
        ILogger<HealthCheckProvider> logger,
        IHealthCheckContextBuilder builder)
    {
        _pipelineWrapper = pipelineWrapper;
        _logger = logger;
        _builder = builder;
    }

    public virtual HealthCheckResult Calculate(HealthCheckData value)
    {
        var context = CreateContext(value);
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

    private IHealthCheckContext CreateContext(HealthCheckData value)
    {
        var context = _builder.Create();
        context.HealthCheckResult = default(HealthCheckResult);
        context.HealthCheckData = value;
        return context;
    }
}
