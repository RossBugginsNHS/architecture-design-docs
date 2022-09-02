namespace dhc;

public readonly record struct  BloodPressure(double Systolic, double Diastolic)
{
    public readonly string BloodPressureDescription => BloodPressureResultConverter.GetResult(Systolic, Diastolic);
}
