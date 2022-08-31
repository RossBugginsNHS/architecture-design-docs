using System.Net;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Polly;
using Polly.Contrib.WaitAndRetry;

public class ClusterClientHostedService : IHostedService
{
    public IClusterClient Client { get; }
    IOptions<OrleansConnection> _orleansConfig;

    ILogger<ClusterClientHostedService> _logger;
    public ClusterClientHostedService(ILogger<ClusterClientHostedService> logger, ILoggerProvider loggerProvider, IOptions<OrleansConnection> orleansConfig)
    {
        _logger = logger;
        _orleansConfig = orleansConfig;

        //var hostEntry = Dns.GetHostEntryAsync("host.docker.internal");
        logger.LogInformation("Getting IP from DNS for {hostName}", _orleansConfig.Value.Host);
        var hostEntry = Dns.GetHostEntry(_orleansConfig.Value.Host);
        var ips = hostEntry.AddressList;
        var endpoints = new List<IPEndPoint>();
        foreach(var ip in ips)
        {
            logger.LogInformation("Getting IP from DNS for {hostName} of {ip}", _orleansConfig.Value.Host, ip.ToString());
            endpoints.Add(new IPEndPoint(ip, 30000));
        }
        Client = new ClientBuilder()
        // Appropriate client configuration here, e.g.:
        .UseStaticClustering(endpoints.ToArray())

          //.UseLocalhostClustering()
          .Configure<ClusterOptions>(options =>
            {

                options.ClusterId = "dev";
                options.ServiceId = "OrleansBasics";
            })
        .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
     //   .Configure<ConnectionOptions>(o=>
      //  {
      //      o.ConnectionRetryDelay = TimeSpan.FromSeconds(10);
      //      o.OpenConnectionTimeout = TimeSpan.FromSeconds(10);
            
      //  }).Configure<GatewayOptions>(o=>
      //  {
      //      o.
      //  })
        
        .Build();
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
                       .LogWarning(outcome, "Delaying Orleans client connect to Orleans silo due to {message} for {delay}ms, then making retry {retry}.",outcome.Message, timespan.TotalMilliseconds, retryAttempt);
               }
           );

        await policy.ExecuteAsync(async (ct) =>
        {
            await Client.Connect();
            _logger.LogInformation("Connected to Orleans. IsInitilized {IsInitilized}", Client.IsInitialized);
        }, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Client.Close();
        Client.Dispose();
    }
}

