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



    public async virtual Task PushMessage(string exchangeName, string routingKey, object message, CancellationToken cancellationToken = default)
    {
            var exchangeType = "topic";
            await PushMessage(exchangeName, routingKey, message, exchangeType, cancellationToken);
    }

    public async virtual Task PushNotification(string exchangeName, string routingKey, object message, string exchangeType, CancellationToken cancellationToken = default)
    {
        var model = await _model.GetModel(cancellationToken);
        _logger.LogInformation($"PushMessage,routingKey:{routingKey}");

    
        model.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

        var queue = model.QueueDeclare(queue: "",
                                    durable: false,
                                    exclusive: true,
                                    autoDelete: false,
                                    arguments: null);

        model.QueueBind(queue: queue.QueueName,
                              exchange: exchangeName ,
                              routingKey: routingKey);

        var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());

        string msgJson = JsonConvert.SerializeObject(message, jsonSerializerSettings);

        var body = Encoding.UTF8.GetBytes(msgJson);
        model.BasicPublish(exchange: exchangeName,
                                routingKey: routingKey,
                                basicProperties: null,
                                body: body);
    }

    public async virtual Task PushMessage(string exchangeName, string routingKey, object message, string exchangeType, CancellationToken cancellationToken = default)
    {
        var model = await _model.GetModel(cancellationToken);
        _logger.LogInformation($"PushMessage,routingKey:{routingKey}");

    
        model.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

        var queue = model.QueueDeclare(queue: exchangeName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

        model.QueueBind(queue: queue.QueueName,
                              exchange: exchangeName ,
                              routingKey: routingKey);

        var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());

        string msgJson = JsonConvert.SerializeObject(message, jsonSerializerSettings);

        var body = Encoding.UTF8.GetBytes(msgJson);
        model.BasicPublish(exchange: exchangeName,
                                routingKey: routingKey,
                                basicProperties: null,
                                body: body);

    }
}
