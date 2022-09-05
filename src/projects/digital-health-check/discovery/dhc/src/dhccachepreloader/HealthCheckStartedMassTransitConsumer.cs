using System.Text;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UnitsNet.Serialization.JsonNet;

namespace dhc;

public class HealthCheckStartedMassTransitConsumer: IConsumer<HealthCheckCompletedEvent>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<HealthCheckStartedMassTransitConsumer> _logger;

    public HealthCheckStartedMassTransitConsumer(
        ILogger<HealthCheckStartedMassTransitConsumer> logger,
        IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }
    
    public async Task Consume(ConsumeContext<HealthCheckCompletedEvent> context)
    {
        _logger.LogInformation("Received Text: {Text}", context.Message.HealthCheckResult);
        
        var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
        var messageStr = JsonConvert.SerializeObject(context.Message, jsonSerializerSettings);
        var bytes = Encoding.UTF8.GetBytes(messageStr);
        var healthCheckId =context.Message.HealthCheckId;

        _logger.LogInformation("Added HealthCheckCompleteEvent data for {key} to cache", healthCheckId);
        await _cache.SetAsync(healthCheckId.ToString(), bytes, new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSlidingExpiration(TimeSpan.FromSeconds(30)));    
    }
}
