using System.Text.Json.Serialization;
using dhc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Filters;
using dhcapi;
using UnitsNet;
using MediatR;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
.AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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

var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
if (app.Environment.IsDevelopment())
{
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