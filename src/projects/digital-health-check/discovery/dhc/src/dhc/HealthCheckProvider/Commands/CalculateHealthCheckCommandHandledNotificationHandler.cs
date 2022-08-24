namespace dhc;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using UnitsNet.Serialization.JsonNet;

public class CalculateHealthCheckCommandHandledNotificationHandler : INotificationHandler<CalculateHealthCheckCommandHandledNotification>
{
    private static readonly Counter _c_get_health_check = Metrics.CreateCounter("healthcheck_completed_counter", "Health Check Completed");

    IDistributedCache _cache;
    public CalculateHealthCheckCommandHandledNotificationHandler(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task Handle(CalculateHealthCheckCommandHandledNotification notification, CancellationToken cancellationToken)
    {
        _c_get_health_check.Inc();

        var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());

        string msgJson = JsonConvert.SerializeObject(notification.HealthCheckResult, jsonSerializerSettings);
        var bytes = Encoding.UTF8.GetBytes(msgJson);

        await _cache.SetAsync(notification.HealthCheckData.HealthCheckDataId.id.ToString(), bytes);

    }
}