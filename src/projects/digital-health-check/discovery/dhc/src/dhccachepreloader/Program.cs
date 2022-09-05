using System;
using System.Net;
using System.Threading.Tasks;
using dhc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            var o = new RabbitMqSettings();
            hostContext.Configuration.GetSection(RabbitMqSettings.Location).Bind(o);
            x.AddConsumer<HealthCheckStartedMassTransitConsumer>();
            x.UsingRabbitMq((context,cfg) =>
            {
                cfg.Host(o.RabbitHost, (ushort) o.RabbitPort, "/", h => {
                    
                    h.Username(o.RabbitUserName);
                    h.Password(o.RabbitPassword);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

   //     services.AddSingleton<RabbitMqChannel>();
     //   services.AddSingleton<RabbitMqClient>();
    //    services.Configure<RabbitMqSettings>(hostContext.Configuration.GetSection(RabbitMqSettings.Location));        
  //      services.AddHostedService<HealthCheckCompleteRabbitMqListener>();
 //        services.AddHostedService<HealthCheckStartedRabbitMqListener>();
        //services.AddHostedService<DhcCacheLoader>();
        services.AddStackExchangeRedisCache(options =>
     {
         options.Configuration = hostContext.Configuration.GetSection("Redis")["ConnectionString"];
         options.InstanceName = "dhcapi";


     });
    });

var app = builder.Build();

await app.RunAsync();
