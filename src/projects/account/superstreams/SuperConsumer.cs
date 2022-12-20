using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using System.Text;
using System.Buffers;

public class SuperConsumer : IAsyncDisposable
{

    public StreamSystemConfig StreamSystemConfig { get; init; } = SuperDefaults.DefaultConfig();

    public string Stream { get; init; } = "superstreamtest";

    public int NumberOfClients { get; init; } = 5;

    StreamSystem _system;

    List<Consumer> _consumers = new List<Consumer>();

    public async Task Connect()
    {
        _system = await StreamSystem.Create(StreamSystemConfig);
        await ConnectConsumers();
    }

    public async Task ConnectConsumers()
    {
        for (int conIdLoop = 1; conIdLoop <= NumberOfClients; conIdLoop++)
        {
            var consumer = await ConnectConsumer(conIdLoop);
            _consumers.Add(consumer);
        }
    }


    public async Task<Consumer> ConnectConsumer(int consumerId)
    {
        var consumer = await Consumer.Create(
            new ConsumerConfig(_system, Stream)
            {
                Reference = "consumers",
                OffsetSpec = new OffsetTypeFirst(),
                IsSingleActiveConsumer = true,
                IsSuperStream = true,
                MessageHandler = async (sourceStream, consumer, ctx, message) =>
                {
                    await MessageHandle(consumerId, sourceStream, consumer, ctx, message);
                }
            });
        Console.WriteLine($"Consumer {consumerId} created");
        return consumer;
    }

    public virtual async Task MessageHandle(
        int consumerId,
        string sourceStream,
        RawConsumer consumer,
        MessageContext ctx,
        Message message)
    {
        var id = message.Properties.MessageId.ToString();
        Console.WriteLine(
            $"{sourceStream}:\t message: coming from {consumerId}: {sourceStream}\t Id {id}\t data: {Encoding.Default.GetString(message.Data.Contents.ToArray())} - consumed");
        await Task.CompletedTask;
    }


    public async ValueTask DisposeAsync()
    {
        if (_system != null)
            await _system.Close();
        foreach (var consumer in _consumers)
        {
            if (consumer != null)
                await consumer.Close();
        }
    }
}