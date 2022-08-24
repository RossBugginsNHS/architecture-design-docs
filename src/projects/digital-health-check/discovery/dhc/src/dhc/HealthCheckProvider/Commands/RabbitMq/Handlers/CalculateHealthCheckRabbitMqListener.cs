using System;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UnitsNet.Serialization.JsonNet;
using MediatR;

namespace dhc;

    public class CalculateHealthCheckRabbitMqListener : RabbitListener
    {

    private readonly IPublisher _publisher;
        private readonly ILogger<RabbitListener> _logger;

        // Because the Process function is a delegate callback, if you inject other services directly, they are not in the same scope,
        // To call other Service instances here, you can only get instance objects after IServiceProvider CreateScope
        private readonly IServiceProvider _services;

        public CalculateHealthCheckRabbitMqListener(
            IServiceProvider services, 
            IOptions<RabbitMqSettings> options,
            ILogger<RabbitListener> logger,
             IPublisher publisher) : base(options)
        {
            base.RouteKey = "*";
            base.QueueName = "dhc.healthchecks";
            _logger = logger;
            _services = services;
            _publisher = publisher;
        }

        public async override Task<bool> Process(string message)
        {
            var taskMessage = JToken.Parse(message);
            if (taskMessage == null)
            {
                // When false is returned, the message is rejected directly, indicating that it cannot be processed
                return false;
            }
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var xxxService = scope.ServiceProvider.GetRequiredService<IHealthCheckProvider>();

                    var jsonSerializerSettings = new JsonSerializerSettings {Formatting = Formatting.Indented};
                    jsonSerializerSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());

                    var obj = JsonConvert.DeserializeObject<HealthCheckData>(message, jsonSerializerSettings);

                    var result = await xxxService.CalculateAsync(obj);
                            // var result = await _healthCheckProvider.CalculateAsync(request.HealthCheckData);
                    await _publisher.Publish(new CalculateHealthCheckCommandHandledNotification(obj, result));
                    return true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Process fail,error:{ex.Message},stackTrace:{ex.StackTrace},message:{message}");
                _logger.LogError(-1, ex, "Process fail");
                return false;
            }

        }
    }
