namespace dhc;

public class PipelineRunner<T, CntxtTp> : IPipelineRunner<T, CntxtTp> where T : IHandlingInvoker<CntxtTp>
{
        private static readonly Counter _pipeline_runner_count =
    Metrics.CreateCounter("pipeline_runner_count", "how many times pipeline has run.",
     new CounterConfiguration
     {
         // Here you specify only the names of the labels.
         LabelNames = new[] { "invoker_type_name", "context_type_name" }
     });


    Type _invokerType;
    Type _contextType;
    object locker = new object();
    ContextDelegate<CntxtTp> app;
    IPipelineBuilder<T, CntxtTp> _builder;
    ILogger<PipelineRunner<T, CntxtTp>> _logger;

    public PipelineRunner(
        IPipelineBuilder<T, CntxtTp> builder,
        ILogger<PipelineRunner<T, CntxtTp>> logger)
    {
        _invokerType = typeof(T);
        _contextType = typeof(CntxtTp);
        _builder = builder;
        _logger = logger;
    }

    public async Task<CntxtTp> Run(CntxtTp context)
    {
        BuildIfNeeded();

        _logger.LogInformation("Running pipeline application for context type {contextType} and inokerType {invokerType}", _contextType.FullName, _invokerType.FullName);
        try
        {
        await app(context);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Faile to run pipeline.");
            throw;
        }
        _pipeline_runner_count.WithLabels( _contextType.FullName, _invokerType.FullName).Inc();
        _logger.LogInformation("Ran pipeline application for context type {contextType} and inokerType {invokerType}", _contextType.FullName, _invokerType.FullName);
        return context;
    }

    public void BuildIfNeeded()
    {
        lock (locker)
        {
            if (app == null)
            {
                _logger.LogInformation("App needs building for pipeline application for context type {contextType} and inokerType {invokerType}", _contextType.FullName, _invokerType.FullName);
                app = _builder.Build();
                _logger.LogInformation("Built pipeline application for context type {contextType} and inokerType {invokerType}", _contextType.FullName, _invokerType.FullName);
            }
            else
            {
                _logger.LogInformation("App already exists for pipeline application for context type {contextType} and inokerType {invokerType}", _contextType.FullName, _invokerType.FullName);             
            }
                
        }
    }
}
