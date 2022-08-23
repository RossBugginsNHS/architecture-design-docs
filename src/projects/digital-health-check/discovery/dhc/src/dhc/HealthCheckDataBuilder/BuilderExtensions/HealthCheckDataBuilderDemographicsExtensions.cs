namespace dhc;

public static class HealthCheckDataBuilderDemographicsExtensions
{

    public static readonly string SexAtBirthName = "SexAtBirth"; 

        public static IHealthCheckDataBuilder SexAtBirth(this IHealthCheckDataBuilder builder, string sexAtBirth)
    {
        builder.KeyValue(SexAtBirthName, sexAtBirth);
        return builder;
    }

    public static IHealthCheckDataBuilder Age(this IHealthCheckDataBuilder builder, int days)
    {
        builder.KeyValue("AgeDays", days);
        return builder;
    }

    public static IHealthCheckDataBuilder Postcode(this IHealthCheckDataBuilder builder, string postcode)
    {
        builder.KeyValue("Postcode", postcode);
        return builder;
    }    
}   
            
