using System.Text.Json.Serialization;
using dhc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Filters;
using dhcapi;
using UnitsNet;
using MediatR;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using UnitsNet.Serialization.JsonNet;
using Orleans;
using Orleans.Configuration;
using System.Net;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
.AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

builder.Services.AddEndpointsApiExplorer()
.AddVersionedApiExplorer(setup =>
    {
        setup.GroupNameFormat = "'v'VVV";
        setup.SubstituteApiVersionInUrl = true;
    })
.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.DefaultApiVersion = new ApiVersion(0, 2);
    })
.AddSwaggerGen(c =>
    {
        c.MapType<DateOnly>(() => new OpenApiSchema { Type = typeof(string).Name, Default = new OpenApiString("2020-01-01"), Format = "date" });
        var filePath = Path.Combine(System.AppContext.BaseDirectory, "dhcapi.xml");
        c.IncludeXmlComments(filePath);
        c.EnableAnnotations();
    })
.AddSwaggerExamplesFromAssemblyOf<Program>()
.ConfigureOptions<ConfigureSwaggerOptions>()
.AddHealthChecks()
    .AddCheck<SampleHealthCheck>("Sample")
    .ForwardToPrometheus();

builder.Services.AddHealthCheck((config) =>
{
    config.Services.AddValidatorsFromAssemblyContaining<ConvertHealthCheckCommandHandler>();
    config.Services.AddMediatR(typeof(ConvertHealthCheckCommandHandler));
    config.Services.AddTransient<IHealthCheckRequestDataConverterProvider, HealthCheckRequestDataConverterProvider>();
    config
        .AddWebBmiProvider(builder.Configuration)
        .AddPostCodeApi(builder.Configuration);

    config.Services.AddDistributedMemoryCache();

    config.Services.AddSingleton<RabbitMqClient>();
    config.Services.AddHostedService<CalculateHealthCheckRabbitMqListener>();
});


builder.Services.Configure<OrleansConnection>(builder.Configuration.GetSection(OrleansConnection.Position));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.Location));
builder.Services.Configure<EventStoreSettings>(builder.Configuration.GetSection(EventStoreSettings.Position));

builder.Services.AddSingleton<ClusterClientHostedService>();
builder.Services.AddSingleton<IHostedService>(sp => sp.GetService<ClusterClientHostedService>());
builder.Services.AddSingleton<IClusterClient>(sp => sp.GetService<ClusterClientHostedService>().Client);
builder.Services.AddSingleton<IGrainFactory>(sp => sp.GetService<ClusterClientHostedService>().Client);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOrigin", builder =>
    {
        builder
            .AllowAnyOrigin()
            .WithMethods("PUT", "POST", "GET");
    });
});



var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
if (app.Environment.IsDevelopment())
{
    app.UseCors("AnyOrigin");
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var versionDescriptions = provider
                .ApiVersionDescriptions
                .OrderByDescending(desc => desc.ApiVersion)
                .ToList();

        foreach (var description in versionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
        options.RoutePrefix = "";
    }
    );
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoints =>
    {
        endpoints.MapMetrics();
    });
app.MapHealthChecks("/healthz");
app.UseHttpMetrics();
app.Run();


public class ClusterClientHostedService : IHostedService
{
    public IClusterClient Client { get; }
    IOptions<OrleansConnection> _orleansConfig;

    public ClusterClientHostedService(ILogger<ClusterClientHostedService> logger, ILoggerProvider loggerProvider, IOptions<OrleansConnection> orleansConfig)
    {
        _orleansConfig = orleansConfig;

        //var hostEntry = Dns.GetHostEntryAsync("host.docker.internal");
        logger.LogInformation("Getting IP from DNS for {hostName}", _orleansConfig.Value.Host);
        var hostEntry = Dns.GetHostEntry(_orleansConfig.Value.Host);
        var ip = hostEntry.AddressList[0];
        logger.LogInformation("Getting IP from DNS for {hostName} of {ip}", _orleansConfig.Value.Host, ip.ToString());

        Client = new ClientBuilder()
        // Appropriate client configuration here, e.g.:
        .UseStaticClustering(new IPEndPoint(ip, 30000))

          //.UseLocalhostClustering()
          .Configure<ClusterOptions>(options =>
            {

                options.ClusterId = "dev";
                options.ServiceId = "OrleansBasics";
            })
        .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
        .Build();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // A retry filter could be provided here.
        await Client.Connect();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Client.Close();

        Client.Dispose();
    }
}

