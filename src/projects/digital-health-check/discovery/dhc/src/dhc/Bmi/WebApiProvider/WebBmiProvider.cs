
namespace dhc;



public class WebBmiProvider : IBmiCalculatorProvider
{
    bmiclient.IBmiClient _client;
    IOptions<BmiWebApiSettings> _options;
    ILogger<WebBmiProvider> _logger;
    public WebBmiProvider(bmiclient.IBmiClient client, IOptions<BmiWebApiSettings> options, ILogger<WebBmiProvider> logger)
    {
        _options = options;
        _client = client;
        _logger = logger;
    }
    public Bmi BmiDescription(decimal bmi)
    {
        throw new NotImplementedException();
    }

    public async Task<Bmi> CalculateBmi(Length height, Mass mass)
    {
        _logger.LogInformation("Before calling web bmi api");
        var r = await _client.GetBmiAsync(height.Meters, mass.Kilograms);
        _logger.LogInformation("After calling web bmi api");
        return new Bmi((decimal)r.Bmi);
    }
}
