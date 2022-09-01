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
using Microsoft.AspNetCore.HttpLogging;

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

builder.Services.AddTransient<IHealthCheckDataBuilder, HealthCheckDataBuilder>();
builder.Services.AddTransient<IHealthCheckDataBuilderProvider, HealthCheckDataBuilderProvider>();
var options = new HealthCheckProviderOptions(builder.Services);
options.HealthCheckDataBuilders.Add<HealthCheckDataBuilderBuildFilterId>();
options.HealthCheckDataBuilders.Add<HealthCheckDataBuilderBuildFilterBasicObs>();
options.HealthCheckDataBuilders.Add<HealthCheckDataBuilderBuildFilterDemographics>();
options.HealthCheckDataBuilders.Add<HealthCheckDataBuilderBuildFilterBloodPressure>();
options.HealthCheckDataBuilders.Add<HealthCheckDataBuilderBuildFilterSmoking>();
options.HealthCheckDataBuilders.Add<HealthCheckDataBuilderBuildFilterCholesterol>();
builder.Services.AddHealthCheckHealthCheckDataBuilders(options);
builder.Services.AddValidatorsFromAssemblyContaining<ConvertHealthCheckCommandHandler>();
builder.Services.AddMediatR(typeof(ConvertHealthCheckCommandHandler));
builder.Services.AddMediatR(typeof(CalculateHealthCheckCommandHandlerRabbitMq));
builder.Services.AddTransient<IHealthCheckRequestDataConverterProvider, HealthCheckRequestDataConverterProvider>();

builder.Services.AddSingleton<RabbitMqChannel>();
builder.Services.AddSingleton<RabbitMqClient>();


builder.Services.AddStackExchangeRedisCache(options =>
 {
     options.Configuration = builder.Configuration.GetSection("Redis")["ConnectionString"];
     options.InstanceName = "dhcapi";
 });

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.Location));
builder.Services.Configure<EventStoreSettings>(builder.Configuration.GetSection(EventStoreSettings.Position));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOrigin", builder =>
    {
        builder
            .AllowAnyOrigin()
            .WithMethods("PUT", "POST", "GET");
    });
});

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;

});



var app = builder.Build();

app.UseForwardedHeaders();
app.UseHttpLogging();
app.UseForwardedPrefixBasePath();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AnyOrigin");
    app.UseRelativePathBaseSwaggerAndVersioning();
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

