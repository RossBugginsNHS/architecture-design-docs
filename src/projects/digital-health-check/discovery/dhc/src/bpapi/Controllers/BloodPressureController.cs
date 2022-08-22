using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace bpapi.Controllers;

[ApiController]
[ApiVersion("0.1")]
[ApiVersion("0.2")]
[Route("/v{version:apiVersion}/[controller]")]
public class BloodPressureController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<BloodPressureController> _logger;
    private readonly IBloodPressureProvider _bpProvider;
    public BloodPressureController(
        ISender sender,
        ILogger<BloodPressureController> logger, 
        IBloodPressureProvider bpProvider)
    {
        _sender = sender;
        _logger = logger;
        _bpProvider = bpProvider;
    }

    /// <summary>
    /// Retrieves a Blood Pressure result
    /// </summary>
    /// <remarks>Awesomeness!</remarks>
    /// <param name="systolic" example="100">The systolic (top)</param>
    /// <param name="diastolic" example="75">The diastolic (bottom)</param>
    /// <response code="200" example="Normal">BP result retrieved</response>
    /// <example>Normal 1234</example>
    [Produces("text/plain")]
    [HttpGet("{systolic}/{diastolic}", Name = "GetBloodPressure"), MapToApiVersion("0.1")]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<ActionResult<string>> Get(double systolic, double diastolic)
    {
        await Task.Yield();
        var result = BloodPressureResultConverter.GetResult(systolic, diastolic);
        _logger.LogTrace("V0.1 BP Result of {result} for {systolic} over {diastolic}", result, systolic, diastolic);
        return result;
    }
    

    /// <param name="systolic" example="100"></param>
    /// <param name="diastolic" example="99"></param>
    [SwaggerOperation(
        Summary = "Retrieves a Blood Pressure result.",
        Description = "Returns a text representation of the Blood Pressure results from the provided values.",
        OperationId = "GetBpResult",
        Tags = new[] { "BloodPressure" })]
    [SwaggerResponse(StatusCodes.Status200OK, "The blood pressure result was returned", typeof(BloodPressureResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "An error generating the blood pressure result.")]
    [Produces("application/json")]
    [HttpGet("{systolic}/{diastolic}", Name = "GetBloodPressure"), MapToApiVersion("0.2")]
    public async Task<ActionResult<BloodPressureResult>> GetV2(
        [SwaggerParameter("The systolic (top)", Required = true)]int systolic, 
        [SwaggerParameter("The diastolic (bottom)", Required = true)]int diastolic)
    {
        var bpResult = await _sender.Send(new CalculateBloodPressureCommand(new BloodPressureObservation(systolic, diastolic, DateOnly.FromDateTime(DateTime.Now), "No Location")));

        //var bpResult = _bpProvider.CalculateBloodPressure(systolic,diastolic);
        _logger.LogTrace("V0.2 BP Result of {result} for {systolic} over {diastolic}", bpResult.BloodPressureDescription, systolic, diastolic);
        if(bpResult.BloodPressureDescription == "Error")
            return BadRequest();

        return new BloodPressureResult(bpResult.BloodPressureDescription, systolic, diastolic );
    }

    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpPost(Name = "GetBloodPressures"), MapToApiVersion("0.2")]
    public  ActionResult<BloodPressureResult> GetResult(IEnumerable<BloodPressureObservation> bps)
    {
        var targetList = bps
          .Select(x => _bpProvider.CalculateBloodPressure(x.Systolic, x.Diastolic));

        var bpResult = _bpProvider.CalculateBloodPressure(targetList);

        if(bpResult.BloodPressureDescription == "Error")
            return BadRequest();

        return new BloodPressureResult(bpResult.BloodPressureDescription, (int)Math.Ceiling(bpResult.Systolic), (int)Math.Ceiling(bpResult.Diastolic) );        
    }
}


