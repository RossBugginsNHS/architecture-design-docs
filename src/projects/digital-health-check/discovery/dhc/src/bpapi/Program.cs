using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options=>
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddVersionedApiExplorer(setup =>
    {
        setup.GroupNameFormat = "'v'VVV";
        setup.SubstituteApiVersionInUrl = true;
    });
builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.DefaultApiVersion = new ApiVersion(0, 2);
    });
builder.Services.AddSwaggerGen(c=>
    {
        c.MapType<DateOnly>(() => new OpenApiSchema { Type = typeof(string).Name, Default = new OpenApiString("2020-01-01"), Format="date"});
        var filePath = Path.Combine(System.AppContext.BaseDirectory, "bpapi.xml");
        c.IncludeXmlComments(filePath);
        c.EnableAnnotations();
    });
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddTransient<BloodPressureProvider>();
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
    app.UseSwaggerUI(options=>
    {
          var versionDescriptions = provider
                    .ApiVersionDescriptions
                    .OrderByDescending(desc => desc.ApiVersion)
                    .ToList();

        foreach (var description in versionDescriptions)
       {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant()); 
        }
        options.RoutePrefix ="";
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

app.UseHttpMetrics();
app.Run();


