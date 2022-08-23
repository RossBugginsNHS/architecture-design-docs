using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace dhc;

public class PostCodeHttpClient
{
        private static readonly Counter _c_postcodehttpclient_postcode_lookup_count =
        Metrics.CreateCounter("postcodehttpclient_postcode_lookup_count", "Postcode lookup",
         new CounterConfiguration
         {
             // Here you specify only the names of the labels.
             LabelNames = new[] { "postcode", "cache_hit" }
         });

    HttpClient _client;
    private readonly IDistributedCache  _cache;
    IOptions<PostCodeApiSettings> _settings;
    ILogger<PostCodeHttpClient> _logger;

    public PostCodeHttpClient(HttpClient client, IDistributedCache  cache, IOptions<PostCodeApiSettings> settings, ILogger<PostCodeHttpClient> logger)
    {
        _cache = cache;
        _client = client;
        _settings = settings;
        _logger = logger;
    }

    public async Task<PostcodeInfo> GetAsync(string postcode)
    {
        var stdPostCode = postcode.Replace(" ", "").ToUpper();

        var info = await _cache.GetAsync(stdPostCode);
        if(info==null)
        {
            _c_postcodehttpclient_postcode_lookup_count.WithLabels(stdPostCode, "false").Inc();
            var result = await _client.GetAsync(stdPostCode);
            var str = await result.Content.ReadAsStringAsync();
            var bytes = Encoding.UTF8.GetBytes(str);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<PostcodeInfo>(str);

            await _cache.SetAsync(stdPostCode, bytes, new DistributedCacheEntryOptions()
            {
                SlidingExpiration = _settings.Value.SlidingCacheLength,
                AbsoluteExpirationRelativeToNow = _settings.Value.SlidingCacheLength
            }
            );

            return obj;
        }
        else
        {
            _c_postcodehttpclient_postcode_lookup_count.WithLabels(stdPostCode, "true").Inc();
            var str = Encoding.UTF8.GetString(info);
            _logger.LogDebug("Got from cache key {key} with data {data}", stdPostCode, str);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<PostcodeInfo>(str);
            return obj;
        }

    }
}
