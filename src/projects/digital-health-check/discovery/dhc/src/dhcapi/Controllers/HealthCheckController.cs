using Microsoft.AspNetCore.Mvc;
using dhc;
using Swashbuckle.AspNetCore.Annotations;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using UnitsNet.Serialization.JsonNet;
using System.Text;
using EventStore.Client;
using Microsoft.Extensions.Options;

namespace dhcapi.Controllers;

public readonly record struct HealthCheckRequestDataResponse(Guid Id);
[ApiController]
[ApiVersion("0.1")]
[Route("/v{version:apiVersion}/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly ISender _sender;
    IOptions<EventStoreSettings> _settings;
    IDistributedCache _cache;
    public HealthCheckController(
        ISender sender, IDistributedCache cache,IOptions<EventStoreSettings> settings)
    {
        _sender = sender;
        _cache = cache;
        _settings = settings;
    }

    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpPost(Name = "PostHealthCheck"), MapToApiVersion("0.1")]
    public async Task<ActionResult<HealthCheckRequestDataResponse>> Post(
          [FromBody] HealthCheckRequestData data)
    {
        var healthCheckData = await _sender.Send(new ConvertHealthCheckCommand(data));
        var hcResult = await _sender.Send(new CalculateHealthCheckCommand(healthCheckData));
        return new HealthCheckRequestDataResponse(healthCheckData.HealthCheckDataId.id);
    }

    [Produces("application/json")]
    [HttpGet("{healthCheckId}", Name = "GetHealthCheck"), MapToApiVersion("0.1")]
    public async Task<ActionResult<HealthCheckResult>> Get(
          Guid healthCheckId)
    {
        var result = await _cache.GetAsync(healthCheckId.ToString());

        var settings = EventStoreClientSettings
    .Create(_settings.Value.ConnectionString);
        var client = new EventStoreClient(settings);

        var managementClient = new EventStoreProjectionManagementClient(settings);
        var events = client.ReadStreamAsync(
              Direction.Backwards,
            "healthcheck-" + healthCheckId.ToString(),
                StreamPosition.End);


        if (await events.ReadState == ReadState.Ok)
        {
            var resolved = await events.FirstAsync();
            var str = Encoding.UTF8.GetString(resolved.Event.Data.ToArray());
            var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
            var obj = JsonConvert.DeserializeObject<HealthCheckResult>(str);
            return obj;
        }

        if (result != null)
        {
            var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
            var str = Encoding.UTF8.GetString(result);
            var obj = JsonConvert.DeserializeObject<HealthCheckResult>(str);
            return obj;
        }

        return default(HealthCheckResult);
    }

    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpGet("/v{version:apiVersion}/Tools/DaysOld/{year}/{month}/{day}", Name = "GetBirthdayToDays"), MapToApiVersion("0.1")]
    public async Task<ActionResult<int>> BirthdayToDays(
         [FromRoute] int year,
         [FromRoute] int month,
         [FromRoute] int day
         )
    {
        var birthDate = new DateOnly(year, month, day);
        var today = DateOnly.FromDateTime(DateTime.Now);
        var days = today.DayNumber - birthDate.DayNumber;
        await Task.Yield();
        return days;
    }
}

