using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
namespace dhc;
public static class RelativePathBaseSwaggerAndVersioningExtensionMethods
{
    public static IApplicationBuilder UseRelativePathBaseSwaggerAndVersioning(this IApplicationBuilder app)
    {
        var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger(c =>
    {
        // c.RouteTemplate = "api/{documentName}/swagger.json"; 
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{httpReq.PathBase.Value}"
                }
            };
        });
    });

        app.UseSwaggerUI(options =>
        {
            var versionDescriptions = provider
                      .ApiVersionDescriptions
                      .OrderByDescending(desc => desc.ApiVersion)
                      .ToList();

            foreach (var description in versionDescriptions)
            {
                options.SwaggerEndpoint($"swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
            options.RoutePrefix = "";
        }
        );
        return app;
    }
}