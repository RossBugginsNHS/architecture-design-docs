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

        private readonly IConnection connection;
        private readonly IModel channel;


        public RabbitListener(IOptions<RabbitMqSettings> options)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    // This is my configuration. Just change it to my own use
                    HostName = options.Value.RabbitHost,
                    UserName = options.Value.RabbitUserName,
                    Password = options.Value.RabbitPassword,
                    Port = options.Value.RabbitPort,
                };
                this.connection = factory.CreateConnection();
                this.channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitListener init error,ex:{ex.Message}");
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Register();
        }





        protected string RouteKey;
        protected string QueueName;

        // How to process messages
        public virtual Task<bool> Process(string message)
        {
            throw new NotImplementedException();
        }

        // Registered consumer monitoring here
        public async Task Register()
        {
            Console.WriteLine($"RabbitListener register,routeKey:{RouteKey}");
            channel.ExchangeDeclare(exchange: QueueName, type: "topic");
            channel.QueueDeclare(queue:QueueName, 
                                    autoDelete: false, 
                                    durable: false,
                                    exclusive: false);
            channel.QueueBind(queue: QueueName,
                              exchange: QueueName,
                              routingKey: RouteKey);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var result = await Process(message);
                if (result)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            channel.BasicConsume(queue: QueueName, consumer: consumer);
            await Task.Yield();
            
        }

        public void DeRegister()
        {
            this.connection.Close();
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.connection.Close();
            return Task.CompletedTask;
        }
    }

