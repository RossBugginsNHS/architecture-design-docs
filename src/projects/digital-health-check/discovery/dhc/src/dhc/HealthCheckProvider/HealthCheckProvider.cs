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

    public virtual async Task<HealthCheckResult> CalculateAsync(HealthCheckData value, CancellationToken cancellationToken)
    {
        var context = CreateContext(value, cancellationToken);
        var current = await CalculateResultsAsync(context);
        return current;
    }

    public virtual async Task<HealthCheckResult> CalculateResultsAsync(IHealthCheckContext context)
    {
         _logger.LogDebug("Starting health check calculation on {healthCheckData}", context.HealthCheckData);
        var result = await _pipelineRunner.Run(context);
        LogContextHistory(context);
        _logger.LogDebug("Finished health check calculation on {healthCheckData} with result {healthCheckResult}", context.HealthCheckData, context.HealthCheckResult);
        return context.HealthCheckResult;
    }

    private IHealthCheckContext CreateContext(HealthCheckData value, CancellationToken cancellationToken)
    {
        var context = _builder.Create();
        context.CancellationToken = cancellationToken;
        context.SetHealthCheckResult(default(HealthCheckResult));
        context.SetHealthCheckData(value);
        context.SetHealthCheckResult(context.HealthCheckResult with {HealthCheckResultId = new HealthCheckResultId(value.HealthCheckDataId.id)});
        return context;
    }

    private void LogContextHistory(IHealthCheckContext context)
    {
        foreach(var h in context.GetDataHistory())
        {
            _logger.LogDebug("Context data history {data}", h);
        }

        foreach(var h in context.GetResultsHistory())
        {
            _logger.LogDebug("Context data results {data}", h);
        }        
    }
}
