using dhc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
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
        var filePath = Path.Combine(System.AppContext.BaseDirectory, "bmiapi.xml");
        c.IncludeXmlComments(filePath);
        c.EnableAnnotations();
    });
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddTransient<BmiCalculatorProvider>();

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
