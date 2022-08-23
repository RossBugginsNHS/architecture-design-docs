namespace dhc;

public interface IPipelineRunner<T, CntxtTp> where T : IHandlingInvoker<CntxtTp>
{
    Task<CntxtTp> Run(CntxtTp context);
    void BuildIfNeeded();
}
