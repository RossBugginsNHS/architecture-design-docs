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

        private readonly IModel _channel;

        private readonly ILogger _logger;


        public RabbitMqClient(IOptions<RabbitMqSettings> options, ILogger<RabbitMqClient> logger)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = options.Value.RabbitHost,
                    UserName = options.Value.RabbitUserName,
                    Password = options.Value.RabbitPassword,
                    Port = options.Value.RabbitPort,
                };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                logger.LogError(-1, ex, "RabbitMQClient init fail");
            }
            _logger = logger;
        }

        public virtual void PushMessage(string queueName, string routingKey, object message)
        {
            _logger.LogInformation($"PushMessage,routingKey:{routingKey}");
            _channel.QueueDeclare(queue: queueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);


            var jsonSerializerSettings = new JsonSerializerSettings {Formatting = Formatting.Indented};
            jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());

            string msgJson = JsonConvert.SerializeObject(message, jsonSerializerSettings);

            var body = Encoding.UTF8.GetBytes(msgJson);
            _channel.BasicPublish(exchange: queueName,
                                    routingKey: routingKey,
                                    basicProperties: null,
                                    body: body);

           
        }
    }
