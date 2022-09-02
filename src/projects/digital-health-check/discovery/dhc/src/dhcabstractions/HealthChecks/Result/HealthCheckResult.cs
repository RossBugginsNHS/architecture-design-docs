namespace dhc;

public readonly record struct HealthCheckResultId(Guid Id);
public readonly record struct HealthCheckResult(HealthCheckResultId HealthCheckResultId, HealthCheckDemographics Demographics, BloodPressure BloodPressure, Bmi Bmi, SmokingResult Smoking, CholesterolResult Cholesterol,QriskResult QriskResult, HealthCheckResultGuidance Guidance);
