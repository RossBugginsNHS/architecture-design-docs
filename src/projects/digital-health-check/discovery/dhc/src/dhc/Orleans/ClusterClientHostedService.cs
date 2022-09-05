using System.Net;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Polly;
using Polly.Contrib.WaitAndRetry;

public class ClusterClientHostedService : IHostedService
{
    IClusterClient _client;
    //public IClientBuilder ClientBuilder { get; }
    IOptions<OrleansConnection> _orleansConfig;

    ILogger<ClusterClientHostedService> _logger;
    ILoggerProvider _loggerProvider;
    ClusterClientFactory _clientFactory;

    public ClusterClientHostedService(ILogger<ClusterClientHostedService> logger, ILoggerProvider loggerProvider, IOptions<OrleansConnection> orleansConfig, ClusterClientFactory clientFactory)
    {
        _loggerProvider = loggerProvider;
        _logger = logger;
        _orleansConfig = orleansConfig;
        _clientFactory = clientFactory;

    }


    public async Task StartAsync(CancellationToken cancellationToken)
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
                       .LogWarning(outcome, "Delaying Orleans client connect to Orleans silo due to {message} for {delay}ms, then making retry {retry}.", outcome.Message, timespan.TotalMilliseconds, retryAttempt);
               }
           );

        await policy.ExecuteAsync(async (ct) =>
        {
            var oleansHostValue = _orleansConfig.Value.Host;
            _logger.LogInformation("Getting IP from DNS for {hostName}", oleansHostValue);
            
            if(string.IsNullOrEmpty(oleansHostValue))
                oleansHostValue = Dns.GetHostName();
            var hostEntry = Dns.GetHostEntry(oleansHostValue);
            var ips = hostEntry.AddressList;
            var endpoints = new List<IPEndPoint>();
            foreach (var ip in ips)
            {
                _logger.LogInformation("Getting IP from DNS for {hostName} of {ip}", oleansHostValue, ip.ToString());
                endpoints.Add(new IPEndPoint(ip, 30000));
            }


            var clientBuilder = new ClientBuilder()

           .UseStaticClustering(endpoints.ToArray())

           .Configure<ClusterOptions>(options =>
               {

                   options.ClusterId = "dev";
                   options.ServiceId = "OrleansBasics";
               })
           .ConfigureLogging(builder => builder.AddProvider(_loggerProvider));

            var built = clientBuilder.Build();
            await built.Connect();
            _client = built;
            _clientFactory.SetClient(_client);
       
            _logger.LogInformation("Connected to Orleans. IsInitilized {IsInitilized}", _client.IsInitialized);


        }, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.Close();
        _client.Dispose();
    }
}

