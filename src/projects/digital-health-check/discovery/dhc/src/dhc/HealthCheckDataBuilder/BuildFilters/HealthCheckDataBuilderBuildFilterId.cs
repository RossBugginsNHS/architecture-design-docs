namespace dhc;

public class HealthCheckDataBuilderBuildFilterId: IHealthCheckDataBuilderBuildFilter
{
    public  HealthCheckData Filter (HealthCheckData currentOutput, HealthCheckDataBuilderData inputData)
    {        
        return currentOutput with {HealthCheckDataId =  new HealthCheckDataId(
            Guid.NewGuid())};
    }
}
