namespace dhc;

public class HealthCheckDataBuilderBuildFilterBloodPressure: IHealthCheckDataBuilderBuildFilter
{
    public  HealthCheckData Filter (HealthCheckData currentOutput, HealthCheckDataBuilderData inputData)
    {
        return currentOutput with {BloodPressure =  new BloodPressureObservation(
            inputData.GetValue<double>("Systolic"),
            inputData.GetValue<double>("Diastolic"),
            DateOnly.FromDateTime(DateTime.Now),
            "DefaultLocation")};        
    }
}
