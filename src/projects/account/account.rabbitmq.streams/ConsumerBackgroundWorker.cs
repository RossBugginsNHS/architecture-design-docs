using System.Buffers;
using System.Text;
using RabbitMQ.Stream.Client.AMQP;
using RabbitMQ.Stream.Client.Reliable;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Stream.Client;

namespace account.Consumers;

public class AccountCommand
{
    public string CommandName{get;set;}
    public string Data {get;set;}
}

public class AccountCommandConsumerBackgroundWorker : ConsumerBackgroundWorker
{
    public AccountCommandConsumerBackgroundWorker(
        ILogger<AccountCommandConsumerBackgroundWorker> logger,
        RabbitMqStreamConnectionFactory systemConnection,
        IOptions<RabbitMqStreamOptions> options)
    : base(logger, systemConnection, options)
    {

    }

    public override Task MessageHandle(
        string sourceStream,
        RawConsumer consumer,
        MessageContext ctx,
        Message message,
        CancellationToken cancellationToken)
    {
        var data = message.Data.Contents.ToArray();
        var cmd = System.Text.Json.JsonSerializer.Deserialize<AccountCommand>(data);
        return base.MessageHandle(sourceStream, consumer, ctx, message, cancellationToken);
    }
}

public class ConsumerBackgroundWorker : BackgroundService
{
    static Meter s_meter = new Meter("SuperStreamClients.Consumers", "1.0.0");
    static Counter<int> s_messagesReceived = s_meter.CreateCounter<int>("messages-received-count");

    private readonly ILogger<ConsumerBackgroundWorker> _logger;
    private readonly RabbitMqStreamConnectionFactory _systemConnection;
    private readonly IOptions<RabbitMqStreamOptions> _options;
    private readonly Random _random;
    StreamSystem? _streamSystem;
    Consumer? _consumer;

    public ConsumerBackgroundWorker(
        ILogger<ConsumerBackgroundWorker> logger,
        RabbitMqStreamConnectionFactory systemConnection,
        IOptions<RabbitMqStreamOptions> options
    )
    {
        _logger = logger;
        _systemConnection = systemConnection;
        _options = options;
        var optionsValue = _options.Value;
        _random = new Random(optionsValue.RandomSeed);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!IsConsumerEnabled())
            return;

        await TryCreateConnectionAndConsumer(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await StopConsumerAndConnection();
        await base.StopAsync(cancellationToken);
    }

    private async Task StopConsumerAndConnection()
    {
        if (!IsConsumerEnabled())
            return;

        if (_consumer != null)
            await _consumer.Close();

        if (_streamSystem != null)
            await _streamSystem.Close();
    }

    private bool IsConsumerEnabled() => _options.Value.Consumer;

    private async Task CreateConnection(CancellationToken cancellationToken)
    {
        _streamSystem = await _systemConnection.Create(cancellationToken);
    }

    private async Task TryCreateConnectionAndConsumer(CancellationToken cancellationToken)
    {
        try
        {
            await CreateConnectionAndConsumer(cancellationToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Consumer requested to stop.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failure. Consumer worker terminating.");
        }
    }

    private async Task CreateConnectionAndConsumer(CancellationToken cancellationToken)
    {
        await CreateConnection(cancellationToken);
        await CreateConsumer(cancellationToken);
    }

    public async Task<Consumer> CreateConsumer(CancellationToken cancellationToken)
    {
        var options = _options.Value;
        var consumerConfig = CreateConsumerConfig(cancellationToken);
        _consumer = await Consumer.Create(consumerConfig);
        _logger.LogInformation("Consumer created");
        return _consumer;
    }

    private ConsumerConfig CreateConsumerConfig(
        CancellationToken cancellationToken)
    {
        var options = _options.Value;
        return new ConsumerConfig(_streamSystem, options.StreamName)
        {
            IsSuperStream = true,
            IsSingleActiveConsumer = true,
            Reference = options.ConsumerAppReference,
            OffsetSpec = new OffsetTypeNext(),
            MessageHandler = async (sourceStream, consumer, ctx, message) =>
            {
                await TryMessageHandle(sourceStream, consumer, ctx, message, cancellationToken);
            }
        };
    }

    public virtual async Task TryMessageHandle(
        string sourceStream,
        RawConsumer consumer,
        MessageContext ctx,
        Message message,
        CancellationToken cancellationToken)
    {
        try
        {
            await MessageHandle(sourceStream, consumer, ctx, message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Consumer failed handling message.");
        }
    }

    public virtual Task MessageHandle(
        string sourceStream,
        RawConsumer consumer,
        MessageContext ctx,
        Message message,
        CancellationToken cancellationToken)
    {

        UpdateMetrics(String.Empty);
        LogMessageReceived(sourceStream, String.Empty);
        return Task.CompletedTask;
    }

    private void UpdateMetrics(string contextId)
    {
        s_messagesReceived.Add(
            1,
            new KeyValuePair<string, object?>("Host", GetHostName()),
            new KeyValuePair<string, object?>("ContextId", contextId));
    }

    private string GetHostName() => System.Environment.MachineName;

    private int GetCustomerId(Message message)
    {
        var customerIdString = message?.Properties?.MessageId?.ToString() ?? String.Empty;
        var customerId = int.Parse(customerIdString);
        return customerId;
    }

    private void LogMessageReceived(
        string sourceStream,
        string contextId)
    {
        var hostName = GetHostName();
        var customerId = contextId;
        _logger.LogInformation(
            "Message received for {contextId}.",
            customerId);
    }


    private async Task DelayMessageHandling(
        RabbitMqStreamOptions options,
        CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(
                _random.Next(options.ConsumerHandleDelayMin, options.ConsumerHandleDelayMax),
                cancellationToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Handle delay canceled due cancellation requested");
        }
    }
}