namespace account;

using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Stream.Client;

public class RabbitMqStreamConnectionFactory
{
    static Meter s_meter = new Meter("account.RabbitMqStreamConnectionFactory", "1.0.0");
    static Counter<int> s_connectionAttempt = s_meter.CreateCounter<int>("connection-attempt-count");
    static Counter<int> s_connectedCount = s_meter.CreateCounter<int>("connected-count");

    ILogger<RabbitMqStreamConnectionFactory> _logger;
    IOptions<RabbitMqStreamOptions> _options;

    public RabbitMqStreamConnectionFactory(
        ILogger<RabbitMqStreamConnectionFactory> logger,
        IOptions<RabbitMqStreamOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<StreamSystem> Create(CancellationToken cancellationToken)
    {
        try
        {
            var streamSystem = await CreateWithRetry(cancellationToken);
            UpdateConnectedCount();
            return streamSystem;
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Cancelled RabbitMq connection attempts.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to Connect to RabbitMq.");
            throw;
        }
    }

    private void UpdateConnectedCount()
    {
        s_connectedCount.Add(1,
            new KeyValuePair<string,object?>("Host", Environment.MachineName));
    }

    private void UpdateConnectionAttemptCount()
    {
        s_connectionAttempt.Add(1,
            new KeyValuePair<string,object?>("Host", Environment.MachineName));
    }

    private async Task<StreamSystem> CreateWithRetry(CancellationToken cancellationToken)
    {
        var retryTime = _options.Value.RabbitMqConnectionRetryTime;

        return await Policy
            .Handle<StreamSystemInitialisationException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(
                int.MaxValue,
                (count) => retryTime,
                onRetry: async (ex, delay, attempt, context) =>
                {
                    _logger.LogInformation("Connection will retry in {connectionRetry}", delay);
                    await Task.Delay(delay, cancellationToken);
                    _logger.LogInformation("Connection retry {attempt}", attempt);
                })
            .ExecuteAsync(async (ct) => await AttemptCreateConfigAndStreamSystem(ct), cancellationToken);
    }


    private async Task<StreamSystem> AttemptCreateConfigAndStreamSystem(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var options = _options.Value;

        try
        {
            return await CreateConfigAndStreamSystem(cancellationToken);
        }
        catch (StreamSystemInitialisationException ex)
        {
            _logger.LogWarning("Stream System Initialisation Failed to connect to host: {StreamSystemInitialisationException}.", ex.Message);
            throw;
        }
        catch (SocketException ex)
        {
            _logger.LogWarning("Failed getting host entry for {hostName} {message}", options.HostName, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to host.");
            throw;
        }
    }

    private async Task<StreamSystem> CreateConfigAndStreamSystem(CancellationToken cancellationToken)
    {
        UpdateConnectionAttemptCount();
        var config = await GetConnectionConfig();
        var streamSystem = await StreamSystem.Create(config);
        return streamSystem;
    }



    public async Task<StreamSystemConfig> GetConnectionConfig()
    {
        var options = _options.Value;
        _logger.LogInformation("{user} Connecting to {hostname} {virtualHost}", options.UserName, options.HostName, options.VirtualHost);
        var hostEntry = await GetHostEntry();
        _logger.LogInformation("{IpCount} IP Addresses found", hostEntry.AddressList.Length);
        var config = new StreamSystemConfig
        {
            UserName = options.UserName,
            Password = options.Password,
            VirtualHost = options.VirtualHost,
            Endpoints = GetEndpoints(hostEntry, options.StreamPort).ToList()
        };
        return config;
    }

    private async Task<IPHostEntry> GetHostEntry()
    {
        var options = _options.Value;
        return await Dns.GetHostEntryAsync(options.HostName);
    }

    private IEnumerable<EndPoint> GetEndpoints(IPHostEntry hostEntry, int streamPort)
    {
        var endpoints = hostEntry.AddressList.Select(address => new IPEndPoint(address, streamPort));
        foreach (var endpoint in endpoints)
        {
            _logger.LogInformation("RabbitMq endpoint {endpoint}:{port}", endpoint.Address, endpoint.Port);
            yield return (EndPoint)endpoint;
        }
    }
}