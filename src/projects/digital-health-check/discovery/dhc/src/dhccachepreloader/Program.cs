using System;
using System.Net;
using System.Threading.Tasks;
using dhc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<RabbitMqChannel>();
        services.AddSingleton<RabbitMqClient>();
        services.Configure<RabbitMqSettings>(hostContext.Configuration.GetSection(RabbitMqSettings.Location));        
        services.AddHostedService<HealthCheckCompleteRabbitMqListener>();
         services.AddHostedService<HealthCheckStartedRabbitMqListener>();
        //services.AddHostedService<DhcCacheLoader>();
        services.AddStackExchangeRedisCache(options =>
     {
         options.Configuration = hostContext.Configuration.GetSection("Redis")["ConnectionString"];
         options.InstanceName = "dhcapi";


     });
    });

var app = builder.Build();

await app.RunAsync();
