using System;
using System.Net;
using System.Threading.Tasks;
using dhc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EventStore.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using UnitsNet.Serialization.JsonNet;
using Polly.Contrib.WaitAndRetry;
using Polly;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
    {
        services.Configure<EventStoreSettings>(hostContext.Configuration.GetSection(EventStoreSettings.Position));
        services.AddHostedService<DhcCacheLoader>();
        services.AddStackExchangeRedisCache(options =>
     {
         options.Configuration = hostContext.Configuration.GetSection("Redis")["ConnectionString"];
         options.InstanceName = "dhcapi";


     });
    });

var app = builder.Build();
var loggerTest = app.Services.GetService<ILogger<OrleansConnection>>();
loggerTest.LogTrace("Test a trace message 123");
loggerTest.LogInformation("Test an info message 123");
loggerTest.LogWarning("Test a warning message 123");
loggerTest.LogError("Test an error message 123");
await app.RunAsync();

public class DhcCacheLoader : IHostedService
{

    private readonly ILogger<DhcCacheLoader> _logger;
    IOptions<EventStoreSettings> _settings;
    IDistributedCache _cache;

    StreamSubscription _stream;

    public DhcCacheLoader(IDistributedCache cache, IOptions<EventStoreSettings> settings, ILogger<DhcCacheLoader> logger)
    {
        _cache = cache;
        _settings = settings;
        _logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        
        await StartFromAsync(StreamPosition.Start, cancellationToken);
    }

    public async Task StartFromAsync(StreamPosition position, CancellationToken cancellationToken)
    {
        ulong lastCompleted = position.ToUInt64();

        var settings = EventStoreClientSettings
            .Create(_settings.Value.ConnectionString);
        
        var client = new EventStoreClient(settings);

        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 10);

        var policy = Policy.Handle<Exception>()
           .WaitAndRetryAsync(
               delay,
               onRetry: (outcome, timespan, retryAttempt, context) =>
               {
                   context["totalRetries"] = retryAttempt;
                   context["retryWaitTime"] = timespan;
                   _logger
                       .LogWarning(outcome, "Delaying Event store client for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
               }
           );

        await policy.ExecuteAsync(async () =>
        {
              _stream = await client.SubscribeToStreamAsync(
            "$ce-healthcheck",
            position,
            async (sub, evt, ct) =>
        {
                 var delay2 = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 10);

        var policy2 = Policy.Handle<Exception>()
           .WaitAndRetryAsync(
               delay2,
               onRetry: (outcome, timespan, retryAttempt, context) =>
               {
                   context["totalRetries"] = retryAttempt;
                   context["retryWaitTime"] = timespan;
                   _logger
                       .LogWarning(outcome, "Delaying Event store Read Event for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
               }
           );

             await policy2.ExecuteAsync(async () =>
            {
                var resolved = evt;

                if (resolved.Event.EventType == "HealthCheckCompleteEvent")
                {
                    var healthCheckIdStr = resolved.Event.EventStreamId.Replace("healthcheck-", "");
                    var healthCheckId = Guid.Parse(healthCheckIdStr);

                    _logger.LogInformation("Read HealthCheckCompleteEvent for {key} from event store", healthCheckId);
                    var data = resolved.Event.Data.ToArray();
                    var str = Encoding.UTF8.GetString(data);
                    var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
                    jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
                    var obj = JsonConvert.DeserializeObject<HealthCheckResult>(str, jsonSerializerSettings);

                    await _cache.SetAsync(healthCheckId.ToString(), data, new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSlidingExpiration(TimeSpan.FromSeconds(30)));
                    _logger.LogInformation("Added HealthCheckCompleteEvent data for {key} to cache", healthCheckId);
                }
                else if ((resolved.Event.EventType == "HealthCheckStartedEvent"))
                {
                    var healthCheckIdStr = resolved.Event.EventStreamId.Replace("healthcheck-", "");
                    var healthCheckId = Guid.Parse(healthCheckIdStr);          

                    _logger.LogInformation("Read HealthCheckStartedEvent for {key} from event store", healthCheckId);
                    var data = resolved.Event.Data.ToArray();
                    var str = Encoding.UTF8.GetString(data);
                    var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
                    jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
                    var obj = JsonConvert.DeserializeObject<HealthCheckData>(str, jsonSerializerSettings);
                     _logger.LogInformation("Added HealthCheckStartedEvent data for {key} to cache", healthCheckId);
                    await _cache.SetAsync("HealthCheckStartedEvent_"+healthCheckId.ToString(), data, new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSlidingExpiration(TimeSpan.FromSeconds(30)));          

                }

                  lastCompleted = resolved.OriginalEventNumber.ToUInt64();
            });
        }, true, async (sub, reason, ex) =>
        {
            _logger.LogError(ex, "Problem with Event.");
            await StartFromAsync(new StreamPosition(lastCompleted), cancellationToken);
            
        });
        });
        
      
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _stream.Dispose();
        return Task.CompletedTask;
    }
}