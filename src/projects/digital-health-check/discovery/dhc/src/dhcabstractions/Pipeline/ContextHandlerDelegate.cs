namespace dhc;

public delegate Task ContextHandlerDelegate<CntxtTp> (CntxtTp context, ContextDelegate<CntxtTp> next);