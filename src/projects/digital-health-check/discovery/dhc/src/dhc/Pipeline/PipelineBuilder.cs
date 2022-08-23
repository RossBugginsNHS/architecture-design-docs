namespace dhc;

public class PipelineBuilder<T, CntxtTp> : IPipelineBuilder<T, CntxtTp> where T : IHandlingInvoker<CntxtTp>
{

    ContextDelegate<CntxtTp> app;
    public PipelineBuilder(IEnumerable<T> filters)
    {
        foreach (var filter in filters)
            Use(filter);
        app = Build();
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

        var l = new ContextHandler<CntxtTp>();
        ContextDelegate<CntxtTp> first = l.Handle;
        foreach (var x in wares.AsEnumerable().Reverse())
        {
            first = x(first);
        }
        return first;
    }

    public async Task<CntxtTp> Run(CntxtTp context)
    {
        await app(context);
        return context;
    }
}
