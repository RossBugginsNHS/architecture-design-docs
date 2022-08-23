namespace dhc;

public class HealthCheckDataBuilderBuildFilterCholesterol: IHealthCheckDataBuilderBuildFilter
{
    public  HealthCheckData Filter (HealthCheckData currentOutput, HealthCheckDataBuilderData inputData)
    {        
        return currentOutput with {CholesterolData =  new CholesterolObservation(
            inputData.GetValue<double>(HealthCheckDataBuilderCholesterolExtensions.Hdl),
            inputData.GetValue<double>(HealthCheckDataBuilderCholesterolExtensions.NonHDL))};
    }
}
