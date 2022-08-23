namespace dhc;

public interface IPipelineBuilder<CntxtTp>
{
    ContextDelegate<CntxtTp> Build();
    Task<CntxtTp> Run(CntxtTp context);
    void Use(Func<ContextDelegate<CntxtTp>, ContextDelegate<CntxtTp>> middleware);
    void Use(Func<CntxtTp, Func<Task>, Task> middleware);
    void Use<T>();
    void Use(Type filter);
    void Use(IHandlingInvoker<CntxtTp> middleware);
}

public class PipelineBuilder<T, CntxtTp> : IPipelineBuilder<CntxtTp> where T : IHandlingInvoker<CntxtTp>
{

    ContextDelegate<CntxtTp> app;
    public PipelineBuilder(IEnumerable<T> filters)
    {
        foreach (var filter in filters)
            Use(filter);
        app = Build();
    }

    protected List<Func<ContextDelegate<CntxtTp>, ContextDelegate<CntxtTp>>> wares = new List<Func<ContextDelegate<CntxtTp>, ContextDelegate<CntxtTp>>>();

    public void Use(Func<ContextDelegate<CntxtTp>, ContextDelegate<CntxtTp>> middleware)
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
        var middleware = (IHandlingInvoker<CntxtTp>)Activator.CreateInstance<T>();

        this.Use(next =>
        {
            return context =>
        {
            return middleware.Handle(context, next);
        };
        });
    }

    public void Use(Type filter)
    {
        var middleware = (IHandlingInvoker<CntxtTp>)Activator.CreateInstance(filter);

        this.Use(next =>
        {
            return context =>
        {
            return middleware.Handle(context, next);
        };
        });
    }

    public void Use(IHandlingInvoker<CntxtTp> middleware)
    {
        this.Use(next =>
        {
            return context =>
        {
            return middleware.Handle(context, next);
        };
        });
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
