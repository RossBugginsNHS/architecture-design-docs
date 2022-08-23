using Microsoft.AspNetCore.Mvc;

namespace tdsapi.Controllers;

[ApiController]
[Route("[controller]")]
public class TdsController : ControllerBase
{

    private readonly ILogger<TdsController> _logger;
    TdsDataProvider _provider;

    public TdsController(ILogger<TdsController> logger, TdsDataProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    [HttpGet("{geoCode}", Name = "GetTds")]
    public ActionResult<TdsController> Get(string geoCode)
    {
        return Ok(_provider.Data[geoCode]);
    }
}
