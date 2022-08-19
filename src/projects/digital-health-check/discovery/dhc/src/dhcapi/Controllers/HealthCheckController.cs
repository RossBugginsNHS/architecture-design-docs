using Microsoft.AspNetCore.Mvc;
using dhc;
using Swashbuckle.AspNetCore.Annotations;
using MediatR;
namespace dhcapi.Controllers;

[ApiController]
[ApiVersion("0.1")]
[Route("/v{version:apiVersion}/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly ISender _sender;
    public HealthCheckController(
        ISender sender)
    {
        _sender = sender;
    }

    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpPost(Name = "GetHealthCheck"), MapToApiVersion("0.1")]
    public async Task<ActionResult<HealthCheckResult>> Get(
          [FromBody]HealthCheckRequestData data)
    {
        var healthCheckData = await _sender.Send(new ConvertHealthCheckCommand(data));
        var hcResult = await _sender.Send(new CalculateHealthCheckCommand(healthCheckData));
        return hcResult;
    }

    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpGet("/v{version:apiVersion}/Tools/DaysOld/{year}/{month}/{day}",Name = "GetBirthdayToDays"), MapToApiVersion("0.1")]
    public async Task<ActionResult<int>> BirthdayToDays(
         [FromRoute] int year,
         [FromRoute] int month,
         [FromRoute] int day
         )
    {
        var birthDate = new DateOnly(year, month, day);
        var today = DateOnly.FromDateTime(DateTime.Now);
        var days = today.DayNumber - birthDate.DayNumber;
        await Task.Yield();
        return days;
    }    
}

