using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace dhc;

    public class RabbitListener : IHostedService
    {

       
        private readonly RabbitMqChannel _model;
        private readonly ILogger<RabbitListener> _logger;


        public RabbitListener(RabbitMqChannel model, ILogger<RabbitListener> logger)
        {
            _logger = logger;
            _model = model;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Register(cancellationToken);
        }

        protected string RouteKey;
        protected string QueueName;

        // How to process messages
        public virtual Task<bool> Process(string message,  CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        // Registered consumer monitoring here
        public async Task Register(CancellationToken cancellationToken)
        {
            var channel = await _model.GetModel(cancellationToken);
            _logger.LogInformation("RabbitListener register,routeKey:{RouteKey}", RouteKey);
            var exchangeType = "topic";
            channel.ExchangeDeclare(exchange: QueueName, type: exchangeType);
            _logger.LogInformation("RabbitListener exchange declared, Exchange:{exchange}, Type{exchangeType}", QueueName, exchangeType);
            channel.QueueDeclare(queue:QueueName, 
                                    autoDelete: false, 
                                    durable: false,
                                    exclusive: false);
            _logger.LogInformation("RabbitListener queue declared, QueueName:{QueueName}", QueueName);                              
            channel.QueueBind(queue: QueueName,
                              exchange: QueueName,
                              routingKey: RouteKey);
            _logger.LogInformation("RabbitListener queue bind, QueueName:{QueueName}  Exchange:{exchange}, RouteKey {routingKey}", QueueName, QueueName, RouteKey);     
            
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var result = await Process(message, cancellationToken);
                if (result)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            channel.BasicConsume(queue: QueueName, consumer: consumer);
            await Task.Yield();
            
        }

   

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _model.Close();
            return Task.CompletedTask;
        }
    }

