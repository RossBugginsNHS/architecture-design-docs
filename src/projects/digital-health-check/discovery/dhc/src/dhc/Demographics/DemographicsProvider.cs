namespace dhc;

public class DemographicsProvider
{
    PostCodeHttpClient _client;
    TdsDataProvider _tds;
    public DemographicsProvider(PostCodeHttpClient client, TdsDataProvider tds)
    {
        _tds = tds;
        _client = client;
    }
    public async Task<HealthCheckDemographics> CalculateAsync(HealthCheckData data, HealthCheckDemographics demographics)
    {
        var obj = await _client.GetAsync(data.Demographics.Postcode.Value.Replace(" ", ""));
        var tsd = _tds.Data[obj.Result.Codes.LSOA];
        var tdsVal = tsd.Tds;
        var newDemo = demographics with { Lsoa = obj.Result.Codes.LSOA, Tds = tsd.Tds };
        return newDemo;

    }
}
