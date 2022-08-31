using System;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;
using UnitsNet.Serialization.JsonNet;

namespace dhc;

public class RabbitMqClient
{
    RabbitMqChannel _model;
    public RabbitMqClient(RabbitMqChannel model, ILogger<RabbitMqClient> logger)
    {
        _model = model;
        _logger = logger;
    }


    private readonly ILogger _logger;
    private readonly IOptions<RabbitMqSettings> _options;




    public async virtual Task PushMessage(string queueName, string routingKey, object message, CancellationToken cancellationToken = default)
    {
        var model = await _model.GetModel(cancellationToken);
        _logger.LogInformation($"PushMessage,routingKey:{routingKey}");
        model.QueueDeclare(queue: queueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);


        var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());

        string msgJson = JsonConvert.SerializeObject(message, jsonSerializerSettings);

        var body = Encoding.UTF8.GetBytes(msgJson);
        model.BasicPublish(exchange: queueName,
                                routingKey: routingKey,
                                basicProperties: null,
                                body: body);

    }
}
