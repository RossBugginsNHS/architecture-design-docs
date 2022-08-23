namespace dhc;
public delegate ContextDelegate<CntxtTp> ContextHandlerChainDelegate<CntxtTp>(ContextDelegate<CntxtTp> next);