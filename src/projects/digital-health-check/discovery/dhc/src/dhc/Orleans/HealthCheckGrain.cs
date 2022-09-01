using System.Text;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using UnitsNet.Serialization.JsonNet;

namespace dhc;

public class HealthCheckGrain : Orleans.Grain, IHealthCheckGrain
{
    private readonly ILogger _logger;

    private IHealthCheckProvider _provider;

    IOptions<EventStoreSettings> _settings;
    public HealthCheckGrain(ILogger<HealthCheckGrain> logger, IHealthCheckProvider provider, IOptions<EventStoreSettings> settings)
    {
        _logger = logger;
        _provider = provider;
        _settings = settings;
    }

    public HealthCheckData Data { get; set; }

    public Task AddData(HealthCheckData data)
    {
        Data = data;
        _logger.LogInformation("Add Data message received: data = {data}", data);
        return Task.FromResult($"\n Client said: '{data}', so HelloGrain says: Hello!");
    }

    public async Task Calculate(GrainCancellationToken cancellationToken)
    {
        
        using var _ = cancellationToken.CancellationToken.Register(() => 
             _logger.LogInformation("Grain execution has been requested to be cancelled."));

        try
        {
            var data = Data;
            await StartedEvent(data, cancellationToken.CancellationToken);
            _logger.LogInformation("Grain starting its calculations");
            var result = await _provider.CalculateAsync(data, cancellationToken.CancellationToken);
            _logger.LogInformation("Grain has done all its calculations");
            await CompletedEvent(result, cancellationToken.CancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "failed when running grain");
            throw;
        }
    }

    public async Task StartedEvent(HealthCheckData data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Grain will now create event");
        var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());

        string msgJson = JsonConvert.SerializeObject(data, jsonSerializerSettings);
        var bytes = Encoding.UTF8.GetBytes(msgJson);

        var settings = EventStoreClientSettings
            .Create(_settings.Value.ConnectionString);
        var client = new EventStoreClient(settings);


        var eventData = new EventData(
            Uuid.NewUuid(),
            "HealthCheckStartedEvent",
            bytes);

        await client.AppendToStreamAsync(
    "healthcheck-" + Data.HealthCheckDataId.id.ToString(),
    StreamState.Any,
    new[] { eventData },
    cancellationToken: cancellationToken);

    _logger.LogInformation("Grain has created event");
    }


  public async Task CompletedEvent(HealthCheckResult result, CancellationToken cancellationToken = default)
    {
       
        _logger.LogInformation("Grain will now create event");
        var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());

        string msgJson = JsonConvert.SerializeObject(result, jsonSerializerSettings);
        var bytes = Encoding.UTF8.GetBytes(msgJson);

        var settings = EventStoreClientSettings
            .Create(_settings.Value.ConnectionString);
        var client = new EventStoreClient(settings);


        var eventData = new EventData(
            Uuid.NewUuid(),
            "HealthCheckCompleteEvent",
            bytes);

        await client.AppendToStreamAsync(
    "healthcheck-" + Data.HealthCheckDataId.id.ToString(),
    StreamState.Any,
    new[] { eventData },
    cancellationToken: cancellationToken);

    _logger.LogInformation("Grain has created event");
    }    
}