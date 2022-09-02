namespace dhc;

public readonly record struct BloodPressureObservation(double Systolic, double Diastolic, DateOnly DateTaken, string LocationTaken); 