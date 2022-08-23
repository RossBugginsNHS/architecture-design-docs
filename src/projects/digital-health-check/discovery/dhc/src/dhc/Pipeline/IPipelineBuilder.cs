namespace dhc;

public interface IPipelineBuilder<T, CntxtTp> where T : IHandlingInvoker<CntxtTp>
{
    ContextDelegate<CntxtTp> Build();
    Task<CntxtTp> Run(CntxtTp context);
    void Use(ContextHandlerChainDelegate<CntxtTp> middleware);
    void Use(Func<CntxtTp, Func<Task>, Task> middleware);
    void Use<T>();
    void Use(Type filter);
    void Use(IHandlingInvoker<CntxtTp> middleware);
}
