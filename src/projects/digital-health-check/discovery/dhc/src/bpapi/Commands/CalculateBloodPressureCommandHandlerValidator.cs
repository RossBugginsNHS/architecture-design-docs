using FluentValidation;

namespace bpapi;

public class CalculateBloodPressureCommandHandlerValidator : AbstractValidator<CalculateBloodPressureCommand>
{
    public CalculateBloodPressureCommandHandlerValidator()
    {
        RuleFor(x=> x.RequestData.Systolic).GreaterThan(x=>x.RequestData.Diastolic);
    }
}