namespace dhc;
using FluentValidation;

public class CalculateHealthCheckCommandValidator : AbstractValidator<CalculateHealthCheckCommand>
{
    public CalculateHealthCheckCommandValidator()
    {
        RuleFor(x => x.HealthCheckData.BloodPressure.Systolic).GreaterThan(0);
        RuleFor(x => x.HealthCheckData.BloodPressure.Diastolic).GreaterThan(0);
        RuleFor(x => x.HealthCheckData.BloodPressure.Systolic).GreaterThan(x => x.HealthCheckData.BloodPressure.Diastolic)
            .WithMessage(x => $"Blood Pressure Systolic value ({ x.HealthCheckData.BloodPressure.Systolic}) must be higher than Diastolic value ({x.HealthCheckData.BloodPressure.Diastolic})");
        RuleFor(x => x.HealthCheckData.BasicObs.Height.Meters).GreaterThan(0);
        RuleFor(x => x.HealthCheckData.BasicObs.Waist.Meters).GreaterThan(0);
        RuleFor(x => x.HealthCheckData.BasicObs.Mass.Kilograms).GreaterThan(0);

        RuleFor(x=>x.HealthCheckData.CholesterolData.Hdl).GreaterThan(0);
        RuleFor(x=>x.HealthCheckData.CholesterolData.NonHdl).GreaterThan(0);
    }
}