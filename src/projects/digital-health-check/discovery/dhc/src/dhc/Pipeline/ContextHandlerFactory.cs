namespace dhc;

public class ContextHandlerFactory<CntxtTp> : IContextHandlerFactory<CntxtTp>
{
    IServiceProvider _sp;
    public ContextHandlerFactory(IServiceProvider sp)
    {
        _sp = sp;
    }
    public ContextHandler<CntxtTp> Create()
    {
        return ActivatorUtilities.GetServiceOrCreateInstance<ContextHandler<CntxtTp>>(_sp);
    }
}