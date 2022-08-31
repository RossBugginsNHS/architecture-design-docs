
using System;
using System.Net;
using System.Threading.Tasks;
using dhc;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
    {

        services.AddHealthCheck((config) =>
        {

            config
                .AddWebBmiProvider(hostContext.Configuration)
                .AddPostCodeApi(hostContext.Configuration);

            config.Services.AddDistributedMemoryCache();

            config.Services.AddSingleton<RabbitMqClient>();
        });

        services.Configure<OrleansConnection>(hostContext.Configuration.GetSection(OrleansConnection.Position));
        services.Configure<RabbitMqSettings>(hostContext.Configuration.GetSection(RabbitMqSettings.Location));
        services.Configure<EventStoreSettings>(hostContext.Configuration.GetSection(EventStoreSettings.Position));
    })
    .UseOrleans((hostContext, builder) =>
    {
        
        var orleansConnection = new OrleansConnection();
        hostContext.Configuration.GetSection(OrleansConnection.Position).Bind(orleansConnection);
        
        var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        var ip = hostEntry.AddressList[0];

        var primarySiloEndpoint = new IPEndPoint(ip, 11112);

        builder.UseDevelopmentClustering(primarySiloEndpoint)
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "OrleansBasics";
        })
        .ConfigureEndpoints(siloPort: 11112, gatewayPort: 30000)
        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HealthCheckGrain).Assembly).WithReferences())
        .ConfigureLogging(logging => logging.AddConsole());
    });


var app = builder.Build();
var loggerTest = app.Services.GetService<ILogger<OrleansConnection>>();
loggerTest.LogTrace("Test a trace message 123");
loggerTest.LogInformation("Test an info message 123");
loggerTest.LogWarning("Test a warning message 123");
loggerTest.LogError("Test an error message 123");
await app.RunAsync();

