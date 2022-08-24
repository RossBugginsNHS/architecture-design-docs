namespace dhc;

public interface IBmiCalculatorProvider
{
    Bmi BmiDescription(decimal bmi);
    Task<Bmi> CalculateBmi(Length height, Mass mass);
}
