namespace dhc;

public interface IContextHandlerFactory<CntxtTp>
{
    ContextHandler<CntxtTp> Create();
}
