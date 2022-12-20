using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using System.Text;
using RabbitMQ.Stream.Client.AMQP;

public class SuperProducer : IAsyncDisposable
{
    public StreamSystemConfig StreamSystemConfig { get; init; } = SuperDefaults.DefaultConfig();

    public string Stream { get; init; } = "superstreamtest";

    StreamSystem _system;
    Producer _producer;

    public async Task Connect()
    {
        _system = await StreamSystem.Create(StreamSystemConfig);

        var r = new Random();

        _producer = await Producer.Create(new ProducerConfig(_system, Stream)
        {
            SuperStreamConfig = new SuperStreamConfig()
            {
                Routing = message1 => message1.Properties.MessageId.ToString()
            }
        });

        Console.WriteLine("Producer Created");

        for (var i = 0; i < 1000; i++)
        {
            var custId = r.NextInt64(0, 100);
            var message = new Message(Encoding.Default.GetBytes($"customer {custId}: hello{i}"))
            {
                Properties = new Properties() { MessageId = $"Customer-{custId}" }
            };
            await _producer.Send(message);
            Console.WriteLine($"Written Message {i}");
            await Task.Delay(1000);
        }

    }

    public async ValueTask DisposeAsync()
    {
        if (_system != null)
            await _system.Close();
        if (_producer != null)
            await _producer.Close();
    }
}
