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
        c.MapType<DateOnly>(() => new OpenApiSchema { Type = typeof(string).Name, Default = new OpenApiString("2020-01-01"), Format="date"});        
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
    config.SetWebBpProvider(builder.Configuration);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOrigin", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod();
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