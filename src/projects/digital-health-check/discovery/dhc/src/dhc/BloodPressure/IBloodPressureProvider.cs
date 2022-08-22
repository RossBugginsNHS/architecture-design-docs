namespace dhc;

public interface IBloodPressureProvider
{
    BloodPressure CalculateBloodPressure(double systolic, double diastolic);
    BloodPressure CalculateBloodPressure(IEnumerable<BloodPressure> bloodPressures);
}
