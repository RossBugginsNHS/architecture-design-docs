using System.Text;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UnitsNet.Serialization.JsonNet;

namespace dhc;

public class OrleansConnection
{
    public static readonly string Position = "OrleansConnection";
    public string Host{get;set;}
}

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

    public async Task Calculate(CancellationToken cancellationToken = default)
    {
           _logger.LogInformation("Grain starting its calculations");
        var result = await _provider.CalculateAsync(Data);
        _logger.LogInformation("Grain has done all its calculations");

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