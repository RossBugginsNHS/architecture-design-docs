namespace dhc;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using UnitsNet.Serialization.JsonNet;

public class EventStoreSettings
{
    public static readonly string Position = "EventStoreClient";
    public string ConnectionString{get;set;}
}
public class CalculateHealthCheckCommandHandledNotificationHandler : INotificationHandler<CalculateHealthCheckCommandHandledNotification>
{
    private static readonly Counter _c_get_health_check = Metrics.CreateCounter("healthcheck_completed_counter", "Health Check Completed");

    IDistributedCache _cache;
    IOptions<EventStoreSettings> _settings;
    public CalculateHealthCheckCommandHandledNotificationHandler(IDistributedCache cache, IOptions<EventStoreSettings> settings)
    {
        _cache = cache;
        _settings = settings;
    }
    public async Task Handle(CalculateHealthCheckCommandHandledNotification notification, CancellationToken cancellationToken)
    {
        _c_get_health_check.Inc();

        var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());

        string msgJson = JsonConvert.SerializeObject(notification.HealthCheckResult, jsonSerializerSettings);
        var bytes = Encoding.UTF8.GetBytes(msgJson);

        var settings = EventStoreClientSettings
            .Create(_settings.Value.ConnectionString);
        var client = new EventStoreClient(settings);

            
        var eventData = new EventData(
            Uuid.NewUuid(),
            "HealthCheckCompleteEvent",
            bytes);
        
            await client.AppendToStreamAsync(
        "healthcheck-" + notification.HealthCheckData.HealthCheckDataId.id.ToString(),
        StreamState.Any,
        new[] { eventData },
        cancellationToken: cancellationToken);
        

       // await _cache.SetAsync(notification.HealthCheckData.HealthCheckDataId.id.ToString(), bytes);

    }
}