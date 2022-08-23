
namespace dhc;

public static class HealthCheckDataBuilderCholesterolExtensions
{
    public static readonly string Hdl = "CholesterolHDL";
    public static readonly string NonHDL = "CholesterolNonHDL";

    public static IHealthCheckDataBuilder CholesterolHDL(this IHealthCheckDataBuilder builder, double hdl)
    {
        builder.KeyValue(Hdl, hdl);
        return builder;
    }

    public static IHealthCheckDataBuilder CholesterolNonHDL(this IHealthCheckDataBuilder builder, double nonhdl)
    {
        builder.KeyValue(NonHDL, nonhdl);
        return builder;
    }

}            
