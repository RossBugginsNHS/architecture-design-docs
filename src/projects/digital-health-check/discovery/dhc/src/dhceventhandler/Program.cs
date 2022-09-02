using System;
using System.Net;
using System.Threading.Tasks;
using dhc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EventStore.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using UnitsNet.Serialization.JsonNet;
using Polly.Contrib.WaitAndRetry;
using Polly;
using RabbitMQ.Client;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
    {
  

        services.AddSingleton<RabbitMqChannel>();
        services.AddSingleton<RabbitMqClient>();
        services.Configure<RabbitMqSettings>(hostContext.Configuration.GetSection(RabbitMqSettings.Location));

        services.Configure<EventStoreSettings>(hostContext.Configuration.GetSection(EventStoreSettings.Position));
        services.AddHostedService<HealthCheckEventHandler>();
    
    });

var app = builder.Build();
await app.RunAsync();