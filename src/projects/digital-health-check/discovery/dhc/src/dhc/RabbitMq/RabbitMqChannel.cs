using RabbitMQ.Client;
using Polly.Contrib.WaitAndRetry;
using Polly;

namespace dhc;

public class RabbitMqChannel
{

    private readonly SemaphoreSlim locko = new SemaphoreSlim(1, 1);
    private readonly ILogger<RabbitMqChannel> _logger;
    private readonly IOptions<RabbitMqSettings> _options;

    IModel _model;
    IConnection _connection;

    public RabbitMqChannel(IOptions<RabbitMqSettings> options, ILogger<RabbitMqChannel> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<IModel> GetModel(CancellationToken cancellationToken)
    {
        await locko.WaitAsync(cancellationToken);
        try
        {
            if (_model == null)
                await Connect(cancellationToken);
        }
        finally
        {
            locko.Release();
        }

        return _model;
    }

    private async Task Connect(CancellationToken cancellationToken)
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 10);

        var policy = Policy.Handle<Exception>()
           .WaitAndRetryAsync(
               delay,
               onRetry: (outcome, timespan, retryAttempt, context) =>
               {
                   context["totalRetries"] = retryAttempt;
                   context["retryWaitTime"] = timespan;
                   _logger
                       .LogWarning("Delaying Rabbit mq client connect to rabbit mq for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
               }
           );

        await policy.ExecuteAsync(() =>
        {
            ConnectWithOutRetry();
            return Task.CompletedTask;
        });
    }

    private void ConnectWithOutRetry( )
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _options.Value.RabbitHost,
                UserName = _options.Value.RabbitUserName,
                Password = _options.Value.RabbitPassword,
                Port = _options.Value.RabbitPort,
            };
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            _logger.LogInformation("Connected to rabbit mq");
        }
        catch (Exception ex)
        {
            _logger.LogError(-1, ex, "RabbitMQClient init fail");
            throw;
        }
    }

        public void Close()
        {
            this._connection.Close();
        }
}
