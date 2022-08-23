namespace dhc;

public readonly record struct HealthCheckResult(HealthCheckDemographics Demographics, BloodPressure BloodPressure, Bmi Bmi, SmokingResult Smoking, CholesterolResult Cholesterol,QriskResult QriskResult, HealthCheckResultGuidance Guidance);
