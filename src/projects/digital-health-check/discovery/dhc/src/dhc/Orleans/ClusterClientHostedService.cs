using System.Net;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Polly;
using Polly.Contrib.WaitAndRetry;

public class ClusterClientHostedService : IHostedService
{

    public IClusterClient Client { get; private set; }
    //public IClientBuilder ClientBuilder { get; }
    IOptions<OrleansConnection> _orleansConfig;

    ILogger<ClusterClientHostedService> _logger;
    ILoggerProvider _loggerProvider;

    public ClusterClientHostedService(ILogger<ClusterClientHostedService> logger, ILoggerProvider loggerProvider, IOptions<OrleansConnection> orleansConfig)
    {
        _loggerProvider = loggerProvider;
        _logger = logger;
        _orleansConfig = orleansConfig;

        //var hostEntry = Dns.GetHostEntryAsync("host.docker.internal");




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
            Client = built;
            _logger.LogInformation("Connected to Orleans. IsInitilized {IsInitilized}", Client.IsInitialized);


        }, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Client.Close();
        Client.Dispose();
    }
}

