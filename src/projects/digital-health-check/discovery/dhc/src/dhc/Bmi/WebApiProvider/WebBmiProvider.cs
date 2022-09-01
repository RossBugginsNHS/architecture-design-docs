
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

    public async Task<Bmi> CalculateBmi(Length height, Mass mass, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Before calling web bmi api with {height}m and {weight}kg",height.Meters, mass.Kilograms);
        var r = await _client.GetBmiAsync(height.Meters, mass.Kilograms, cancellationToken);
        _logger.LogInformation("After calling web bmi api with {height}m and {weight}kg and result of {bmi} ({bmiDescription}) bmi",height.Meters, mass.Kilograms, r.Bmi, r.BmiDescription);
        return new Bmi((decimal)r.Bmi, Enum.Parse<BmiEnum>(r.BmiDescription));
    }
}
