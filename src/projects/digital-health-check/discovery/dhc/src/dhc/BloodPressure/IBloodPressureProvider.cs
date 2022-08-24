namespace dhc;

public interface IBloodPressureProvider
{
    Task<BloodPressure> CalculateBloodPressure(double systolic, double diastolic);
    BloodPressure CalculateBloodPressure(IEnumerable<BloodPressure> bloodPressures);
}
