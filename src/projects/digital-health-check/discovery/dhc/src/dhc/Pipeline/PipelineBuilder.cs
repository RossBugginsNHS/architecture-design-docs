namespace dhc;

public class PipelineBuilder<T, CntxtTp> : IPipelineBuilder<T, CntxtTp> where T : IHandlingInvoker<CntxtTp>
{
    Type _invokerType;
    Type _contextType;
    IContextHandlerFactory<CntxtTp> _contextFactory;
    ContextDelegate<CntxtTp> app;
    ILogger<PipelineBuilder<T, CntxtTp>> _logger;
    public PipelineBuilder(
        IEnumerable<T> filters, 
        ILogger<PipelineBuilder<T, CntxtTp>> logger,
        IContextHandlerFactory<CntxtTp> contextFactory)
    {
        _invokerType = typeof(T);
        _contextType= typeof(CntxtTp);
        _logger = logger;
        _contextFactory = contextFactory;
        foreach (var filter in filters)
            Use(filter);
    }

    protected List<ContextHandlerChainDelegate<CntxtTp>> wares = new List<ContextHandlerChainDelegate<CntxtTp>>();

    public void Use(ContextHandlerChainDelegate<CntxtTp> middleware)
    {
        wares.Add(middleware);
    }

    public void Use(Func<CntxtTp, Func<Task>, Task> middleware)
    {
        this.Use((next) =>
        {
            return (context) =>
            {
                Func<Task> simpleNext = () => next(context);
                return middleware(context, simpleNext);
            };

        });
    }

    public void Use<T>()
    {
        Use(typeof(T));
    }

    public void Use(Type filter)
    {
        var middleware = (IHandlingInvoker<CntxtTp>)Activator.CreateInstance(filter);
         _logger.LogInformation("Adding pipeline {pipelineName} to application for context type {contextType} and inokerType {invokerType}",middleware.GetType().FullName, _contextType.FullName, _invokerType.FullName);
        Use(middleware);
    }

    public void Use(IHandlingInvoker<CntxtTp> middleware)
    {
        //Types not needed, but in for clearer code explanation.
        ContextHandlerChainDelegate<CntxtTp>  nextChain = 
        (next) =>
        {
            ContextDelegate<CntxtTp> thisHandler = 
            (context) =>
            {
                ContextHandlerDelegate<CntxtTp> handleTask = middleware.Handle;
                return handleTask(context, next);
            };
            return thisHandler;
        };

        this.Use(nextChain);
    }

    public ContextDelegate<CntxtTp> Build()
    {
        _logger.LogInformation("Building pipeline application for context type {contextType} and inokerType {invokerType}", _contextType.FullName, _invokerType.FullName);
        var contextHandler = _contextFactory.Create();
        _logger.LogInformation("Building pipeline application for context type {contextType} and inokerType {invokerType} adding handler of {handlerName}", _contextType.FullName, _invokerType.FullName, contextHandler.GetType().FullName);
        ContextDelegate<CntxtTp> first = contextHandler.Handle;
        int pipelineCount = 0;
        foreach (var x in wares.AsEnumerable().Reverse())
        {
            pipelineCount ++;
             _logger.LogInformation("Building pipeline application for context type {contextType} and inokerType {invokerType} adding pipeline delegate {pipelineCount}",  _contextType.FullName, _invokerType.FullName, pipelineCount);
            first = x(first);
        }
        _logger.LogInformation("Built pipeline application for context type {contextType} and inokerType {invokerType} with  {pipelineCount} pipeline delegates",  _contextType.FullName, _invokerType.FullName, pipelineCount);
        return first;
    }
}
