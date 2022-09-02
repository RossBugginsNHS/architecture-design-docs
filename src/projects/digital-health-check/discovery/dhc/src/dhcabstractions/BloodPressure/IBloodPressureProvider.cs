namespace dhc;

public interface IBloodPressureProvider
{
    Task<BloodPressure> CalculateBloodPressure(double systolic, double diastolic,  CancellationToken cancellationToken);
    BloodPressure CalculateBloodPressure(IEnumerable<BloodPressure> bloodPressures);
}
