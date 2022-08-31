using System.Reflection;
using dhc;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;


// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddSingleton<RabbitMqChannel>();
        services.AddHostedService<CalculateHealthCheckRabbitMqListener>();
        services.Configure<OrleansConnection>(hostContext.Configuration.GetSection(OrleansConnection.Position));
        services.Configure<RabbitMqSettings>(hostContext.Configuration.GetSection(RabbitMqSettings.Location));
        services.AddSingleton<ClusterClientHostedService>();
        services.AddSingleton<IHostedService>(sp => sp.GetService<ClusterClientHostedService>());
        services.AddSingleton<IClusterClient>(sp => sp.GetService<ClusterClientHostedService>().Client);
        services.AddSingleton<IGrainFactory>(sp => sp.GetService<ClusterClientHostedService>().Client);
});

var app = builder.Build();
var loggerTest = app.Services.GetService<ILogger<OrleansConnection>>();
loggerTest.LogTrace("Test a trace message 123");
loggerTest.LogInformation("Test an info message 123");
loggerTest.LogWarning("Test a warning message 123");
loggerTest.LogError("Test an error message 123");
await app.RunAsync();
