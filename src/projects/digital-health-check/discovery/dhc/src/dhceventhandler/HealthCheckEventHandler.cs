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
using MassTransit;

public class HealthCheckEventHandler : IHostedService
{

    RabbitMqClient _client;
    private readonly ILogger<HealthCheckEventHandler> _logger;
    IOptions<EventStoreSettings> _settings;
    readonly IBus _bus;
    StreamSubscription _stream;

    public HealthCheckEventHandler(
        IOptions<EventStoreSettings> settings, 
        ILogger<HealthCheckEventHandler> logger, 
        RabbitMqClient client,
        IBus bus)
    {
        _client = client;
        _settings = settings;
        _logger = logger;
        _bus = bus;
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
                FromStream.After(position),
                async (sub, evt, ct) =>
                {
                    lastCompleted = await HandleEvent(evt);

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


    public async Task<ulong> HandleEvent(ResolvedEvent evt)
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

        return await policy2.ExecuteAsync(async () =>
        {
            return await HandleEventWithOutRetry(evt);
        });
    }

    public async Task<ulong> HandleEventWithOutRetry(ResolvedEvent evt)
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
         //   await _client.PushMessage("dhc.healthcheckcomplete", healthCheckId.ToString(), obj, "fanout");
            
            await _bus.Publish(new HealthCheckCompletedEvent(){
                HealthCheckId = healthCheckId, 
                HealthCheckResult = obj
            });

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
        //    await _client.PushMessage("dhc.healthcheckstarted", healthCheckId.ToString(), obj, "fanout");
        }

        return resolved.OriginalEventNumber.ToUInt64();
    }
}
