using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace dhcapi;


/// <summary>
/// Data for a Health Check.
/// </summary>
/// <param name="AgeDays">Age in days</param>
/// <param name="MassKg">Body Mass (weight) in kg</param>
/// <param name="HeightM">Height in meters</param>
/// <param name="WaistM">Waist in meters</param>
/// <param name="Diastolic">Required Systolic.</param>
/// <param name="Systolic">Required Systolic.</param>
/// <param name="Systolic2">Optional second Systloic.</param>
/// <param name="Diastolic2">Optional second Diastolic.</param>
/// <param name="Systolic3">Optional third Systloic.</param>
/// <param name="Diastolic3">Optional third Diastolic.</param>
/// <param name="Postcode">UK Postcode</param>
/// <param name="SmokePerDay">Number smoked per day now.</param>
/// <param name="UsedToSmoke">If used to smoke</param>
/// <param name="CholesterolHDL"></param>
/// <param name="CholesterolNonHDL"></param>
[SwaggerSchemaFilter(typeof(HealthCheckRequestDataSchemaFilter))]
public record HealthCheckRequestData(
    //Note, bug with Model Validation, if this is a record struct validation attributes are ignored and validation allways passes.
    [Range(14600, int.MaxValue, ErrorMessage = "Please enter a value bigger than or equal to {1}")]
    int AgeDays, 
    double MassKg, 
    double HeightM, 
    double WaistM, 

    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than or equal to {1}")]
    int Systolic,

    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than or equal to {1}")]  
    int Diastolic, 

    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than or equal to {1}")]  
    int? Systolic2, 

    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than or equal to {1}")]  

    int? Diastolic2, 
    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than or equal to {1}")]      
    int? Systolic3, 

    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than or equal to {1}")]      
    int? Diastolic3, 
    string Postcode, 
    int SmokePerDay,
    bool UsedToSmoke,
    double CholesterolHDL,
    double CholesterolNonHDL,
    SexAtBirthEnum SexAtBirth);




