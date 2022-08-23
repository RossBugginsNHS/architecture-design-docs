namespace dhc;

public class HealthCheckDataBuilderBuildFilterDemographics: IHealthCheckDataBuilderBuildFilter
{
 

    public  HealthCheckData Filter (HealthCheckData currentOutput, HealthCheckDataBuilderData inputData)
    {
        var result = currentOutput with {Demographics =  new HealthCheckDemographicData(
             new Age(inputData.GetValue<int>("AgeDays")), 
             new Postcode(inputData.GetValue<string>("Postcode")),
             Enum.Parse<SexAtBirthEnum>(inputData.GetValue<string>(HealthCheckDataBuilderDemographicsExtensions.SexAtBirthName)))};

        return result;
    }
}
