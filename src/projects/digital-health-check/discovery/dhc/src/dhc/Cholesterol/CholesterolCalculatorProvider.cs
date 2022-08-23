public class CholesterolCalculatorProvider:ICholesterolCalculatorProvider
{
    private readonly ILogger<CholesterolCalculatorProvider> _logger;
    
    public  CholesterolCalculatorProvider( ILogger<CholesterolCalculatorProvider> logger)
    {
        _logger = logger;
    }

    public CholesterolResult Calculate(CholesterolObservation obs)
    {
        _logger.LogInformation("Calculating cholesterol result for {CholesterolObservation}", obs);
        var ratio = obs.Hdl / obs.NonHdl;
        var result = new CholesterolResult(ratio);
        _logger.LogInformation("Calculated cholesterol result of {CholesterolResult} for {CholesterolObservation}", result, obs);
        return result;
    }
}
