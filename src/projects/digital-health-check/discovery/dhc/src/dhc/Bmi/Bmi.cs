namespace dhc;

public readonly record struct Bmi(decimal BmiValue, BmiEnum BmiDescription)
{
    //public readonly BmiEnum BmiDescription => BmiResultConverter.GetResult(BmiValue);
}
