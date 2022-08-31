
namespace dhc;



public class WebBpProvider : IBloodPressureProvider
{
    bpclient.IBpClient _client;
    IOptions<BpWebApiSettings> _options;
    ILogger<WebBpProvider> _logger;
    public WebBpProvider(bpclient.IBpClient client, IOptions<BpWebApiSettings> options, ILogger<WebBpProvider> logger)
    {
        _options = options;
        _client = client;
        _logger = logger;
    }


    public async Task<BloodPressure> CalculateBloodPressure(double systolic, double diastolic)
    {
        _logger.LogInformation("Before calling web bp api");
        var r = await _client.GetBpResultAsync((int)systolic, (int)diastolic);
        _logger.LogInformation("After calling web bp api");
        return new BloodPressure(r.Systolic, r.Diastolic);       
    }

    public BloodPressure CalculateBloodPressure(IEnumerable<BloodPressure> bloodPressures)
    {
        throw new NotImplementedException();
    }

 
}
