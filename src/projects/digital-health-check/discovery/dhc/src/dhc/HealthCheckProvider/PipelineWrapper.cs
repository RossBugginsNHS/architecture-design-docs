namespace dhc;

public class PipelineWrapper2<T>: PipelineWrapper<T>  where T: IHandlingInvoker
{
    public PipelineWrapper2(IEnumerable<T> filters):base(filters)
    {
       
    }
}

public class PipelineWrapper<T>  where T: IHandlingInvoker
{

    HealthCheckResultDelegate app;
    public PipelineWrapper(IEnumerable<T> filters)
    {
        foreach (var filter in filters)
            Use(filter);
        app = Build();
    }

    List<Func<HealthCheckResultDelegate, HealthCheckResultDelegate>> wares = new List<Func<HealthCheckResultDelegate, HealthCheckResultDelegate>>();

    public void Use(Func<HealthCheckResultDelegate, HealthCheckResultDelegate> middleware)
    {
        wares.Add(middleware);
    }

    public void Use(Func<HealthCheckContext, Func<Task>, Task> middleware)
    {
        this.Use(next =>
        {
            return context =>
            {
                Func<Task> simpleNext = () => next(context);
                return middleware(context, simpleNext);
            };
        });
    }
    public void Use<T>() where T : IHandlingInvoker
    {
        var middleware = (IHandlingInvoker)Activator.CreateInstance<T>();

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
        var middleware = (IHandlingInvoker)Activator.CreateInstance(filter);

        this.Use(next =>
        {
            return context =>
        {
            return middleware.Handle(context, next);
        };
        });
    }

    public void Use(IHandlingInvoker middleware)
    {
        this.Use(next =>
        {
            return context =>
        {
            return middleware.Handle(context, next);
        };
        });
    }

    public HealthCheckResultDelegate Build()
    {
        var l = new HealthCheckContextHandler();
        HealthCheckResultDelegate first = l.Handle;
        foreach (var x in wares.AsEnumerable().Reverse())
        {
            first = x(first);
        }
        return first;
    }

    public HealthCheckResult Run(HealthCheckData data)
    {
        HealthCheckContext context = new HealthCheckContext();
        context.HealthCheckResult = default(HealthCheckResult);
        context.HealthCheckData = data;
        app(context);
        return context.HealthCheckResult;
    }
}
