using System.Text;
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
using UnitsNet.Serialization.JsonNet;
using Polly.Contrib.WaitAndRetry;
using Polly;
using RabbitMQ.Client;

public class HealthCheckEventHandler : IHostedService
{

  RabbitMqClient _client;
      private readonly ILogger<HealthCheckEventHandler> _logger;
    IOptions<EventStoreSettings> _settings;

    StreamSubscription _stream;

    public HealthCheckEventHandler(IOptions<EventStoreSettings> settings, ILogger<HealthCheckEventHandler> logger,   RabbitMqClient client)
    {
        _client = client;
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
                    await _client.PushMessage("dhc.healthcheckcomplete", healthCheckId.ToString(), obj, "fanout");
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
                     await _client.PushMessage("dhc.healthcheckstarted", healthCheckId.ToString(), obj, "fanout");
                  

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