using Microsoft.Extensions.Logging;

namespace dhc;

public class BmiCalculatorProvider : IBmiCalculatorProvider
{

    private static readonly Counter _c_calculate_bmi =
        Metrics.CreateCounter("bmi_provider_calculate", "Calculate BMI");

    private static readonly Counter _c_calculate_bmi_with_result =
        Metrics.CreateCounter("bmi_provider_calculate_with_result", "Calculate BMI per result",
         new CounterConfiguration
         {
             // Here you specify only the names of the labels.
             LabelNames = new[] { "bmi_result_description" }
         });

    private readonly ILogger<BmiCalculatorProvider> _logger;

    public BmiCalculatorProvider(ILogger<BmiCalculatorProvider> logger)
    {
        _logger = logger;
    }

    public Task<Bmi> CalculateBmi(Length height, Mass mass)
    {
        _c_calculate_bmi.Inc();
        _logger.LogTrace("Using {bmiProviderName} in{bmiProviderAssemblyName} as BMI Provider", typeof(BmiCalculatorProvider).FullName, typeof(BmiCalculatorProvider).AssemblyQualifiedName);
        _logger.LogDebug("Height {heightcm} cm", height.Centimeters);
        _logger.LogDebug("Mass {massg} g", mass.Grams);
        var heightM = (decimal)height.Meters;
        var massKg = (decimal)mass.Kilograms;
        _logger.LogDebug("Height {heightM} m", heightM);
        _logger.LogDebug("Mass {massKg} kg", massKg);
        _logger.LogTrace("Using {bmiCalculatorName} in{bmiCaclulatorAssemblyName} for BMI Calculation", typeof(BmiCalculator).FullName, typeof(BmiCalculator).AssemblyQualifiedName);
        var bmi = BmiCalculator.Calculate(height, mass);
        _logger.LogDebug("BMI {bmi}", bmi);
        var bmiObj = new Bmi(bmi);
        _c_calculate_bmi_with_result.WithLabels(bmiObj.BmiDescription.ToString()).Inc();
        _logger.LogDebug("BMI value {bmiValue}", bmiObj.BmiValue);
        _logger.LogDebug("BMI result {bmiResult}", bmiObj.BmiDescription);
        return Task.FromResult(bmiObj);
    }

    public Bmi BmiDescription(decimal bmi)
    {
        var bmiObj = new Bmi(bmi);
        _logger.LogDebug("BMI value {bmiValue}", bmiObj.BmiValue);
        _logger.LogDebug("BMI result {bmiResult}", bmiObj.BmiDescription);
        return bmiObj;
    }
}
