using System;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UnitsNet.Serialization.JsonNet;

using Microsoft.Extensions.Caching.Distributed;

namespace dhc;

public class HealthCheckStartedRabbitMqListener : RabbitListener
{

    IDistributedCache _cache;
    private readonly ILogger<HealthCheckStartedRabbitMqListener> _logger;

    // Because the Process function is a delegate callback, if you inject other services directly, they are not in the same scope,
    // To call other Service instances here, you can only get instance objects after IServiceProvider CreateScope
    private readonly IServiceProvider _services;

    public HealthCheckStartedRabbitMqListener(
        IDistributedCache cache,
        IServiceProvider services,
        RabbitMqChannel model,
        ILogger<HealthCheckStartedRabbitMqListener> logger
    ) : base(model, logger)
    {
        base.RouteKey = "*";
        base.QueueName = "healthcheckcompletedhcapiprecache";
        base.ExchangeName = "dhc.healthcheckcomplete";
        base.ExchangeType = "fanout";
        _logger = logger;
        _services = services;
        _cache = cache;
    }

    public async override Task<bool> Process(string message, CancellationToken cancellationToken = default)
    {
        

        var taskMessage = JToken.Parse(message);
        if (taskMessage == null)
        {
            // When false is returned, the message is rejected directly, indicating that it cannot be processed
            return false;
        }

        var bytes = Encoding.UTF8.GetBytes(message);
        var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
        var obj = JsonConvert.DeserializeObject<HealthCheckResult>(message, jsonSerializerSettings);

        var healthCheckId =obj.HealthCheckResultId.Id;

        _logger.LogInformation("Added HealthCheckCompleteEvent data for {key} to cache", healthCheckId);
        await _cache.SetAsync(healthCheckId.ToString(), bytes, new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSlidingExpiration(TimeSpan.FromSeconds(30)));    

        return true;



    }
}
