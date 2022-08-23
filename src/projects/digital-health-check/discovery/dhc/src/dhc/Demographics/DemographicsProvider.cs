namespace dhc;

public class DemographicsProvider
{
  IHttpClientFactory _client;
    TdsDataProvider _tds;
    public DemographicsProvider(IHttpClientFactory client, TdsDataProvider tds)
    {
        _tds = tds;
        _client = client;
    }
    public async Task<HealthCheckDemographics> CalculateAsync(HealthCheckData data, HealthCheckDemographics demographics)
    {
        var postcode = "https://api.postcodes.io/postcodes/";
        var client = _client.CreateClient();
        var result  = await client.GetAsync(postcode+data.Demographics.Postcode.Value.Replace(" ", ""));
        var str = await result.Content.ReadAsStringAsync();
        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<PostcodeInfo>(str);
        var tsd = _tds.Data[obj.Result.Codes.LSOA];
        var tdsVal = tsd.Tds;
        var newDemo = demographics with {Lsoa = obj.Result.Codes.LSOA, Tds = tsd.Tds};
        return newDemo;
       
    }    
}
