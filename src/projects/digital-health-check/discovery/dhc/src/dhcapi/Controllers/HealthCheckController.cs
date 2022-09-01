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
    private readonly ILogger<HealthCheckController> _logger;
    IOptions<EventStoreSettings> _settings;
    IDistributedCache _cache;
    public HealthCheckController(
        ISender sender, IDistributedCache cache, IOptions<EventStoreSettings> settings, ILogger<HealthCheckController> logger)
    {
        _sender = sender;
        _cache = cache;
        _settings = settings;
        _logger = logger;
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult<HealthCheckResult>> Get(
          Guid healthCheckId)
    {
        var result = await _cache.GetAsync(healthCheckId.ToString());

        if (result != null)
        {
            _logger.LogInformation("Got {key} from cache", healthCheckId);
            var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
            var str = Encoding.UTF8.GetString(result);
            var obj = JsonConvert.DeserializeObject<HealthCheckResult>(str);
            return obj;
        }
        else
        {
            _logger.LogWarning("Failed to get {key} from cache", healthCheckId);
        }

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
            _logger.LogInformation("Read some data for {key} from event store", healthCheckId);
            while (await events.MoveNextAsync())
            {
                var resolved = events.Current;
                if (resolved.Event.EventType == "HealthCheckCompleteEvent")
                {
                    _logger.LogInformation("Read HealthCheckCompleteEvent for {key} from event store", healthCheckId);
                    var data = resolved.Event.Data.ToArray();
                    var str = Encoding.UTF8.GetString(data);
                    var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
                    jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
                    var obj = JsonConvert.DeserializeObject<HealthCheckResult>(str);

                    await _cache.SetAsync(healthCheckId.ToString(), data, new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSlidingExpiration(TimeSpan.FromSeconds(30)));
                     _logger.LogInformation("Added HealthCheckCompleteEvent data for {key} to cache", healthCheckId);

                    return obj;
                }
            }
        }

        //check to see if started but not finished
        var started_result = await _cache.GetAsync("HealthCheckStartedEvent_" + healthCheckId.ToString());

        if (started_result != null)
        {
            _logger.LogInformation("Health Check {healthCheckId} has started but not completed", healthCheckId);
            return Accepted();
        }

        _logger.LogInformation("Health Check {healthCheckId} can not be found", healthCheckId);
        
        return NotFound();
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

