using System.Text;

namespace dhc;

public delegate Task HealthCheckResultDelegate(HealthCheckContext context);


public delegate Task ContextDelegate<T>(T context);