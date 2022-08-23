using Microsoft.Extensions.Hosting;

namespace dhc;

public class SetupPipelineHostedService<T, CntxtTp> : IHostedService where T : IHandlingInvoker<CntxtTp>
{
    private readonly IPipelineRunner<T, CntxtTp> _runner;
    private readonly ILogger<SetupPipelineHostedService<T, CntxtTp>> _logger;
    public SetupPipelineHostedService(IPipelineRunner<T, CntxtTp> runner, ILogger<SetupPipelineHostedService<T, CntxtTp>> logger)
    {
        _runner = runner;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Pre building pipeline runner");
        _runner.BuildIfNeeded();
        _logger.LogInformation("Pre built pipeline runner");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
