using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace dhcapi;

public class HealthCheckRequestDataSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = new OpenApiObject
        {
            [ "AgeDays" ] = new OpenApiInteger(14600),            
            [ "MassKg" ] = new OpenApiDouble(100),
            [ "HeightM" ] = new OpenApiDouble(1.8),
            [ "WaistM" ] = new OpenApiDouble(1),
            [ "Systolic" ] = new OpenApiInteger(100),
            [ "Diastolic" ] = new OpenApiInteger(70),
            [ "Postcode" ] = new OpenApiString("SW1W 0NY"),
            [ "SmokePerDay" ] = new OpenApiInteger(0),            
            [ "UsedToSmoke" ] = new OpenApiBoolean(false),
            [ "CholesterolHDL" ] = new OpenApiDouble(10.5),
            [ "CholesterolNonHDL" ] = new OpenApiDouble(2.3), 
        };
    }
}


