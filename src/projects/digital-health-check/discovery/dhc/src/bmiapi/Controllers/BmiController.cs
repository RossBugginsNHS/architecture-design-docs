/*
 * Created Date: Monday, September 12th 2022, 8:18:19 am
 * Author: Ross Buggins (NHS) (78215796+RossBugginsNHS@users.noreply.github.com>)
 * -----
 * Last Modified: 12/09/2022 09:02:51 am
 * Modified By: Ross Buggins (NHS)
 * -----
 * Copyright (c) 2022 Crown Copyright
 * -----
 * GNU General Public License v3.0 or later
 * 
 * This file is part of NHS England Digital Health Check.
 * 
 * NHS England Digital Health Check is free software: you can redistribute it and/or modify it under the terms
 * of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 * 
 * NHS England Digital Health Check is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with NHS England Digital Health Check.
 * If not, see <https://www.gnu.org/licenses/>.
 * -----
 */


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
    //logger
    private readonly ILogger<BmiController> _logger;
    private readonly IBmiCalculatorProvider _bmiProvider;
    public BmiController(
        ILogger<BmiController> logger,
        IBmiCalculatorProvider bmiProvider)
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
        [SwaggerParameter("Weight/Mass (KG)", Required = true)]double weight,
        CancellationToken cancellationToken = default)
    {
        var result = await _bmiProvider.CalculateBmi(Length.FromMeters(height), Mass.FromKilograms(weight), cancellationToken);

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
