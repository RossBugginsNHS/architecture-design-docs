using System.Text;

namespace dhc;

public class HealthCheckProvider : IHealthCheckProvider
{
    private readonly IPipelineRunner<IHealthCheckProviderFilter, IHealthCheckContext> _pipelineRunner;
    private readonly ILogger<HealthCheckProvider> _logger;
    private readonly IHealthCheckContextFactory _builder;

    public HealthCheckProvider(
        IPipelineRunner<IHealthCheckProviderFilter, IHealthCheckContext> pipelineRunner,
        ILogger<HealthCheckProvider> logger,
        IHealthCheckContextFactory builder)
    {
        _pipelineRunner = pipelineRunner;
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
        var result = _pipelineRunner.Run(context);
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
