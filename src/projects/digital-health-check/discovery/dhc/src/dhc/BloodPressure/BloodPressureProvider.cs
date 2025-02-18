using Microsoft.Extensions.Logging;

namespace dhc;

public class BloodPressureProvider : IBloodPressureProvider
{
    private static readonly Counter _c_calculate_bp =
        Metrics.CreateCounter("blood_pressure_provider_calculate", "Calculate BP",
         new CounterConfiguration
         {
             // Here you specify only the names of the labels.
             LabelNames = new[] { "bloodpressure_result_description" }
         });
    private readonly ILogger<BloodPressureProvider> _logger;

    public BloodPressureProvider(ILogger<BloodPressureProvider> logger)
    {
        _logger = logger;
    }

    public BloodPressure CalculateBloodPressure(double systolic, double diastolic)
    {
        ;
        _logger.LogTrace("Calculating BP result for {systolic}/{diastolic}", systolic, diastolic);
        var bp = new BloodPressure(systolic, diastolic);
        _c_calculate_bp.WithLabels(bp.BloodPressureDescription).Inc();
        _logger.LogTrace("Result for BP {systolic}/{diastolic} of {bpResult}", systolic, diastolic, bp.BloodPressureDescription);
        return bp;
    }

    public BloodPressure CalculateBloodPressure(IEnumerable<BloodPressure> bloodPressures)
    {

        return new BloodPressure(
            bloodPressures.Skip(1).Average(a => a.Systolic),
            bloodPressures.Skip(1).Average(a => a.Diastolic));
    }
}
