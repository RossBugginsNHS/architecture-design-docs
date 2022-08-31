using Microsoft.AspNetCore.Mvc;
using dhc;
using UnitsNet;
using Swashbuckle.AspNetCore.Annotations;
using bmiapi;
namespace bmiapi.Controllers;

[ApiController]
[ApiVersion("0.1")]
[ApiVersion("0.2")]
[Route("/v{version:apiVersion}/[controller]")]
public class BmiController : ControllerBase
{
    private readonly ILogger<BmiController> _logger;
    private readonly BmiCalculatorProvider _bmiProvider;
    public BmiController(
        ILogger<BmiController> logger,
        BmiCalculatorProvider bmiProvider)
    {
        _logger = logger;
        _bmiProvider = bmiProvider;
    }   

    [Produces("text/plain")]
    [HttpGet("{bmi}/description", Name = "GetBmiDescription"), MapToApiVersion("0.1")]
    public string Get([FromRoute]decimal bmi)
    {
        var result= BmiResultConverter.GetResult(bmi).ToString();
        _logger.LogTrace("Description of {result} for {bmi}", result, bmi);
        return result;
       
    }

    [Produces("application/json")]
    [HttpGet("{height}/{weight}", Name = "GetBmi"), MapToApiVersion("0.2")]
    public async Task<ActionResult<BmiResult>> GetBmi(
        [SwaggerParameter("Height (Meters)", Required = true)]double height, 
        [SwaggerParameter("Weight/Mass (KG)", Required = true)]double weight)
    {
        var result = await _bmiProvider.CalculateBmi(Length.FromMeters(height), Mass.FromKilograms(weight));

        _logger.LogTrace("Description of {result} for {heightM} m and {weightKg} kg", result.BmiDescription.ToString(), height, weight);
        var bmiResult = new BmiResult(result.BmiValue, result.BmiDescription.ToString());
        
        return bmiResult;
    }

    [Produces("text/plain")]
    [HttpGet("{bmi}/description", Name = "GetBmiDescription"), MapToApiVersion("0.2")]
    public string GetV02([FromRoute]decimal bmi)
    {
        var result= _bmiProvider.BmiDescription(bmi).BmiDescription.ToString();
        _logger.LogTrace("Description of {result} for {bmi}", result, bmi);
        return result;
    }
}
